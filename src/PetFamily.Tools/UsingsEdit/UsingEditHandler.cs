using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.MSBuild;


namespace PetFamily.Tools.UsingsEdit;



public class UsingEditHandler
{
    public Operation? Operation { get; set; }
    public string OldUsing { get; set; } = string.Empty;
    public string NewUsing { get; set; } = string.Empty;
    public string SolutionPath { get; set; }
    public string ClassName { get; set; } = string.Empty;

    public bool IsUsingStatic { get; set; } = false;

    public UsingEditHandler(string[] args)
    {
        SolutionPath = GetSolutionPath();

        var oldNamespaceArgs = args
            .FirstOrDefault(arg => arg.StartsWith("--old_using="))?.Split('=');

        if (oldNamespaceArgs == null || oldNamespaceArgs.Length != 2)
            OldUsing = string.Empty;
        else
        {
            OldUsing = oldNamespaceArgs[1];
        }

        var newNamespaceArgs = args
            .FirstOrDefault(arg => arg.StartsWith("--new_using="))?.Split('=');

        if (newNamespaceArgs == null || newNamespaceArgs.Length != 2)
            NewUsing = string.Empty;
        else
            NewUsing = newNamespaceArgs[1];

        var operationArg = args
           .FirstOrDefault(arg => arg.StartsWith("--remove")
                || arg.StartsWith("--replace")
                || arg.StartsWith("--add"));

        if (Enum.TryParse<Operation>(operationArg?.Replace("-", string.Empty), ignoreCase: true, out var operation))
            Operation = operation;
        else
            throw new Exception($"Operation not found: {operationArg}");

        var classNameArgs = args
           .FirstOrDefault(arg => arg.StartsWith("--class="))?.Split('=');

        if (classNameArgs == null || classNameArgs.Length != 2)
            ClassName = string.Empty;
        else
            ClassName = classNameArgs[1];
    }

    public async Task Handle()
    {
        Console.WriteLine($"### Using edit operation: [{Operation.ToString()}] ###");
        Console.WriteLine($"### Old using: '{OldUsing}' ###");
        Console.WriteLine($"### New using: '{NewUsing}' ###");


        MSBuildLocator.RegisterDefaults();
        using var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(SolutionPath);

        bool updatedAny;

        do
        {
            updatedAny = false;
            Console.WriteLine("#########DO#######");
            Console.WriteLine($"SolutionPath:{SolutionPath}");
            Console.WriteLine($"SolutionProject count:{solution.Projects.Count()}");
            foreach (var project in solution.Projects)
            {
                Console.WriteLine($"############# PROJECT '{project.Name}' ###########");
                foreach (var document in project.Documents)
                {
                    Console.WriteLine($"# d :{document.Name}");
                    var isUsingUpdated = await ProcessDocument(document, workspace);

                    if (isUsingUpdated)
                    {
                        Console.WriteLine($"✅{Operation.ToString()} using in: {document.FilePath} - completed!");
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

        Console.WriteLine("✅ Done.");
    }

    private string GetSolutionPath()
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

    public async Task<bool> ProcessDocument(Document document, MSBuildWorkspace workspace)
    {
        Console.WriteLine("################Proccess###################3");
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

            Console.WriteLine("############## CURRENT NAMESPACE ##############" +
                $"\n\t{currentNamespace}");
            if (currentNamespace == OldUsing.Trim())
            {
                Console.WriteLine("Deleting oldNameSpace");
                editor.RemoveNode(usingDirective);
                Console.WriteLine("AddOldNamespace");
                AddUsing(editor);

                hasChanges = true;
            }
        }

        if (!hasChanges)
            return false;

        var updatedDoc = editor.GetChangedDocument();
        return workspace.TryApplyChanges(updatedDoc.Project.Solution);
    }

    private void AddUsing(DocumentEditor editor)
    {
        var root = editor.OriginalRoot as CompilationUnitSyntax;
        if (root == null) return;

        var alreadyExists = root.Usings
            .Any(u => u.Name.ToString() == NewUsing &&
                      (!IsUsingStatic || u.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)));

        if (alreadyExists) return;

        var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(NewUsing))
            .WithStaticKeyword(IsUsingStatic ? SyntaxFactory.Token(SyntaxKind.StaticKeyword) : default)
            .NormalizeWhitespace()
            .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed);

        var newRoot = root.WithUsings(root.Usings.Insert(0, newUsing));
        editor.ReplaceNode(root, newRoot);
    }

}
public enum Operation
{
    Add,
    Remove,
    Replace
}
