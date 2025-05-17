using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.MSBuild;


namespace PetFamily.Tools.UsingsEdit;

public class UsingEditHandler
{
    public Operation Operation { get; set; }
    public string SolutionPath { get; set; }
    public Dictionary<string, string> ClassUsings { get; set; } = [];
    public UsingEditHandler(string[] args)
    {
        SolutionPath = GetSolutionPath();

        var namespaceArgs = args
            .Where(arg => arg.StartsWith("--usings:"))
            .Select(a => a.Replace("--usings:", string.Empty));

        foreach (var arg in namespaceArgs)
        {
            var parts = arg.Replace(":", " ").Split("%", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                ClassUsings.Add(key, value);
            }
        }

        var operationArg = args
           .FirstOrDefault(arg => arg.StartsWith("--remove", StringComparison.CurrentCultureIgnoreCase)
                || arg.StartsWith("--replace", StringComparison.CurrentCultureIgnoreCase)
                || arg.StartsWith("--add", StringComparison.CurrentCultureIgnoreCase));

        if (Enum.TryParse<Operation>(operationArg?.Replace("-", string.Empty), ignoreCase: true, out var operation))
            Operation = operation;
        else
            throw new Exception($"Operation not found: {operationArg}");
    }

    public async Task Handle()
    {
        Console.WriteLine($"### Using edit operation: [{Operation}] ###");

        MSBuildLocator.RegisterDefaults();
        using var workspace = MSBuildWorkspace.Create();

        var solution = await workspace.OpenSolutionAsync(SolutionPath);
        var currentSolution = solution;
        int updatedCount = 0;

        Console.WriteLine($"SolutionPath: {SolutionPath}");
        Console.WriteLine($"Projects found: {solution.Projects.Count()}");

        foreach (var project in solution.Projects)
        {
            Console.WriteLine($"########## PROJECT: {project.Name} ##########");

            foreach (var document in project.Documents)
            {
                var updatedDocument = await TryProcessDocument(document);
                if (updatedDocument != null)
                {
                    var updatedRoot = await updatedDocument.GetSyntaxRootAsync();
                    if (updatedRoot == null)
                        continue;

                    // ВСТРАИВАЕМ в текущий solution обновлённый root
                    currentSolution = currentSolution.WithDocumentSyntaxRoot(document.Id, updatedRoot);

                    Console.WriteLine($"✅ {Operation} using in: {document.FilePath} - prepared for update");
                    updatedCount++;
                }
            }
        }

        Console.WriteLine($"📦 Applying {updatedCount} document changes...");

        if (updatedCount > 0)
        {
            var success = workspace.TryApplyChanges(currentSolution);
            Console.WriteLine(success ? "✅ Done." : "❌ Failed to apply changes.");
        }
        else
        {
            Console.WriteLine("⚠️ No documents changed.");
        }
    }

    private async Task<Document?> TryProcessDocument(Document document)
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
                : usingDirective.Name?.ToString();

            

            if (ClassUsings.TryGetValue(currentNamespace, out var replacement)==false)
                continue;

            switch (Operation)
            {
                case Operation.Add:
                    Console.WriteLine($"[{Operation}] + {replacement} (based on {currentNamespace})");
                    AddUsing(editor, replacement);
                    break;

                case Operation.Remove:
                    Console.WriteLine($"[{Operation}] - {currentNamespace}");
                    editor.RemoveNode(usingDirective, SyntaxRemoveOptions.KeepNoTrivia);
                    break;

                case Operation.Replace:
                    Console.WriteLine($"[{Operation}] {currentNamespace} -> {replacement}");
                    editor.RemoveNode(usingDirective, SyntaxRemoveOptions.KeepNoTrivia);
                    AddUsing(editor, replacement);
                    break;
            }

            //ClassUsings.Remove(currentNamespace);
            hasChanges = true;
        }

        return hasChanges ? editor.GetChangedDocument() : null;
    }

    private static void AddUsing(DocumentEditor editor, string newUsing)
    {
        var isStatic = newUsing.StartsWith("static ");
        var cleanName = isStatic ? newUsing.Substring(7) : newUsing;

        var root = editor.OriginalRoot as CompilationUnitSyntax;
        if (root == null) return;

        var alreadyExists = root.Usings.Any(u =>
            u.Name.ToString() == cleanName &&
            u.StaticKeyword.IsKind(SyntaxKind.StaticKeyword) == isStatic);

        if (alreadyExists) return;

        var nameSyntax = SyntaxFactory.ParseName(cleanName);
        var newUsingDirective = SyntaxFactory.UsingDirective(nameSyntax)
            .WithStaticKeyword(isStatic ? SyntaxFactory.Token(SyntaxKind.StaticKeyword) : default)
            .NormalizeWhitespace()
            .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed);

        editor.InsertBefore(root.Usings.FirstOrDefault(), newUsingDirective);
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
public enum Operation
{
    Add,
    Remove,
    Replace
}
