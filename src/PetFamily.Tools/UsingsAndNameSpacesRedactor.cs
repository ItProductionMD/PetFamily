using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.MSBuild;

namespace PetFamily.Tools;

public static class UsingsAndNameSpacesRedactor
{
    public static async Task RunUsingOperation(
    string[] args,
    Func<Document, MSBuildWorkspace, Task<(bool updated, string filePath)>> processDocument)
    {
        var pathToSolution = GetSolutionPath();
        Console.WriteLine($"📁 Solution path: {pathToSolution}");

        MSBuildLocator.RegisterDefaults();
        using var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(pathToSolution);

        bool updatedAny;
        do
        {
            updatedAny = false;

            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var (updated, filePath) = await processDocument(document, workspace);
                    if (updated)
                    {
                        Console.WriteLine($"✅ Updated: {filePath}");
                        updatedAny = true;
                        break;
                    }
                }

                if (updatedAny)
                    break;
            }

            if (updatedAny)
            {
                solution = workspace.CurrentSolution;
            }

        } while (updatedAny);

        Console.WriteLine("🎉 Done.");
    }

    public async static Task RunAddUsings(string[] args)
    {
        var myArgs = GetArgumentsForAddUsings(args);
        var pathToSolution = GetSolutionPath();
        Console.WriteLine($"Solution path: {pathToSolution}");

        MSBuildLocator.RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(pathToSolution);

        var targetClass = myArgs.TargetArgs;
        var namespaceToAdd = myArgs.NameSpace;
        var isStatic = myArgs.IsStatic;

        bool updatedAny;
        do
        {
            updatedAny = false;

            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var updated = await ProcessAddDocument(document, workspace, targetClass, namespaceToAdd, isStatic);
                    if (updated)
                    {
                        Console.WriteLine($"✅ Updated: {document.FilePath}");
                        updatedAny = true;
                        break; // выходим из document-цикла
                    }
                }

                if (updatedAny)
                    break; // выходим из project-цикла
            }

            if (updatedAny)
            {
                solution = workspace.CurrentSolution; // пересоздаём solution
            }

        } while (updatedAny);

        Console.WriteLine("✅ Done");
    }

    public async static Task RunRemoveUsings(string[] args)
    {
        var namespacesToRemove = GetNamespacesToRemove(args);
        if (!namespacesToRemove.Any())
            throw new Exception("No --remove arguments provided.");

        foreach (var namespaceToRemove in namespacesToRemove)
            Console.WriteLine($"###Try to delete namespace:{namespaceToRemove}###");

        var pathToSolution = GetSolutionPath();
        Console.WriteLine($"###Solution path: {pathToSolution}###");

        MSBuildLocator.RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(pathToSolution);

        bool updatedAny;

        do
        {
            updatedAny = false;

            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var updated = await ProcessRemoveDocument(document, workspace, namespacesToRemove[0]);
                    if (updated)
                    {
                        Console.WriteLine($"❌ Removed from: {document.FilePath}");
                        updatedAny = true;
                        break;
                    }
                }

                if (updatedAny)
                    break;
            }

            if (updatedAny)
            {
                solution = workspace.CurrentSolution;
            }

        } while (updatedAny);

        Console.WriteLine("✅ Done removing.");
    }

    public async static Task RunReplaceUsings(string[]args)
    {
        var pathToSolution = GetSolutionPath();
        Console.WriteLine($"###Solution path: {pathToSolution}###");
        var namespacesToReplace = GetNamespacesToReplace(args);
        if (namespacesToReplace.Count != 2)
            throw new Exception("Arguments count for replace must be 2!");


        Console.WriteLine($"###Try to replace namespace:{namespacesToReplace[0]} on {namespacesToReplace[1]}###");

        MSBuildLocator.RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(pathToSolution);

        bool updatedAny;

        do
        {
            updatedAny = false;

            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var replaced = await ProcessReplaceDocument(
                        document,
                        workspace,
                        namespacesToReplace[0],
                        namespacesToReplace[1]);

                    if (replaced)
                    {
                        Console.WriteLine($"❌ Namespace was replaced in: {document.FilePath}");
                        updatedAny = true;
                        break;
                    }
                }

                if (updatedAny)
                    break;
            }
            if (updatedAny)
            {
                solution = workspace.CurrentSolution;
            }

        } while (updatedAny);

        Console.WriteLine("✅ Done removing.");
    }

    private static bool NeedsUsing(SyntaxNode root, SemanticModel semanticModel, string targetClass, string namespaceToAdd)
    {
        var identifiers = root.DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .Where(id => id.Identifier.Text == targetClass);

        foreach (var id in identifiers)
        {
            var symbol = semanticModel.GetSymbolInfo(id).Symbol;
            if (symbol == null) continue;

            var ns = symbol.ContainingNamespace?.ToDisplayString();
            if (string.IsNullOrEmpty(ns)) continue;

            var existingUsings = root.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .Select(u => u.Name.ToString());

            if (!existingUsings.Contains(namespaceToAdd))
                return true;
        }

        return false;
    }

    private static void AddUsing(DocumentEditor editor, string namespaceToAdd, bool isStatic = false)
    {
        var root = editor.OriginalRoot as CompilationUnitSyntax;
        if (root == null) return;

        var alreadyExists = root.Usings
            .Any(u => u.Name.ToString() == namespaceToAdd &&
                      (!isStatic || u.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)));

        if (alreadyExists) return;

        var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceToAdd))
            .WithStaticKeyword(isStatic ? SyntaxFactory.Token(SyntaxKind.StaticKeyword) : default)
            .NormalizeWhitespace()
            .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed);

        var newRoot = root.WithUsings(root.Usings.Insert(0, newUsing)); // 👈 Добавляем в начало
        editor.ReplaceNode(root, newRoot);
    }

    private static AllArgs GetArgumentsForAddUsings(string[] args)
    {
        if (args.Length < 3)
            throw new Exception("Arguments count less than three!");

        string targetClass = string.Empty;
        string namespaceToAdd = string.Empty;
        bool isStatic = false;

        foreach (var arg in args.Skip(1))
        {
            var values = arg.Split('=');

            if (arg.StartsWith("--class="))
                targetClass = values.Length == 2 ? values[1] : string.Empty;

            if (arg.StartsWith("--namespace="))
                namespaceToAdd = values.Length == 2 ? values[1] : string.Empty;

            if (arg.StartsWith("--static="))
                isStatic = values.Length == 2
                    ? (values[1] == "true" ? true : false)
                    : false;
        }

        if (string.IsNullOrWhiteSpace(targetClass))
            throw new Exception("Target class argument is empty!");

        if (string.IsNullOrWhiteSpace(namespaceToAdd))
            throw new Exception("Namespace argument is empty!");

        return new(targetClass, namespaceToAdd, isStatic);
    }

    private static string GetArgumentsForDeleteNamespaces(string[] args)
    {
        if (args.Length >10)
            throw new Exception("Arguments count less than three!");

        string namespaceToDelete = string.Empty;
        bool isStatic = false;

        foreach (var arg in args.Skip(1))
        {
            var values = arg.Split('=');

            if (arg.StartsWith("--namespace="))
                namespaceToDelete = values.Length == 2 ? values[1] : string.Empty;
            if (string.IsNullOrWhiteSpace(namespaceToDelete))
                throw new Exception("Namespace argument is empty!");
        }
        return namespaceToDelete;
    }

    private static List<string> GetNamespacesToRemove(string[] args)
    {
        return args
            .Where(arg => arg.StartsWith("--remove="))
            .Select(arg => arg.Split('=')[1].Replace("%"," "))
            .ToList();

    }

    private static List<string> GetNamespacesToReplace(string[] args)
    {
        return args
            .Skip(1)
            .Select(arg => arg.Split('=')[1])
            .ToList();
    }

    private static async Task<bool> ProcessAddDocument(
        Document document,
        MSBuildWorkspace workspace,
        string targetClass,
        string namespaceToAdd,
        bool isStatic)
    {
        var root = await document.GetSyntaxRootAsync();
        var semanticModel = await document.GetSemanticModelAsync();
        var editor = await DocumentEditor.CreateAsync(document);

        if (!NeedsUsing(root, semanticModel, targetClass, namespaceToAdd))
            return false;

        AddUsing(editor, namespaceToAdd, isStatic);

        var updatedDoc = editor.GetChangedDocument();
        return workspace.TryApplyChanges(updatedDoc.Project.Solution);
    }

    private static async Task<bool> ProcessRemoveDocument(
        Document document,
        MSBuildWorkspace workspace,
        string namespaceToRemove)
    {
        var root = await document.GetSyntaxRootAsync();
        if (root == null)
            return false;

        var editor = await DocumentEditor.CreateAsync(document);

        var toRemove = root.DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .Where(u =>
            {
                var fullUsing = u.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                    ? $"static {u.Name}"
                    : u.Name.ToString();

                return fullUsing == namespaceToRemove;
            })
            .ToList();

        if (!toRemove.Any())
            return false;

        foreach (var usingDirective in toRemove)
            editor.RemoveNode(usingDirective);

        var updatedDoc = editor.GetChangedDocument();
        return workspace.TryApplyChanges(updatedDoc.Project.Solution);
    }

    private static async Task<bool> ProcessReplaceDocument(
    Document document,
    MSBuildWorkspace workspace,
    string oldNamespace,
    string newNamespace)
    {
        var root = await document.GetSyntaxRootAsync();
        var editor = await DocumentEditor.CreateAsync(document);

        var hasChanges = false;

        var usings = root.DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .ToList();

        foreach (var usingDirective in usings)
        {
            var currentNamespace = usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                ? $"static {usingDirective.Name}"
                : usingDirective.Name.ToString();

            if (currentNamespace == oldNamespace)
            {
                AddUsing(editor,newNamespace,false);
                hasChanges = true;
            }
        }

        if (!hasChanges)
            return false;

        var updatedDoc = editor.GetChangedDocument();
        return workspace.TryApplyChanges(updatedDoc.Project.Solution);
    }
    private static string GetSolutionPath()
    {
        var baseDir = AppContext.BaseDirectory;
        var dir = new DirectoryInfo(baseDir);

        while (dir != null && !dir.GetFiles("*.sln").Any())
        {
            dir = dir.Parent;
        }

        string pathToFolder = dir?.FullName ?? throw new Exception("Solution directory not found");
        string pathToSolution = Path.Combine(pathToFolder, "PetFamily.sln");
        if (!Path.Exists(pathToSolution))
            throw new FileNotFoundException($"Solution file not found: {pathToSolution}");

        return pathToSolution;
    }
}

public struct AllArgs(string targetClass, string nameSpace, bool isStatic)
{
    public string TargetArgs = targetClass; 
    public string NameSpace = nameSpace;
    public bool IsStatic = isStatic;
}