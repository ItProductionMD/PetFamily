using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
namespace PetFamily.Tools.DatabaseManagement;

public static class DropAndResetDatabaseV2
{
    public static async Task RunAsync(string solutionPath)
    {
        Console.WriteLine("Run DropAndResetDatabase...");

        var workspace = MSBuildWorkspace.Create();
        Console.WriteLine("workspace created");

        var solution = await workspace.OpenSolutionAsync(solutionPath);
        Console.WriteLine($"solution opened!Path:{solution.FilePath}");

        var startupProjectPath = Path.Combine(Path.GetDirectoryName(solutionPath)!, "src", "PetFamily.Host.Api", "PetFamily.Host.Api.csproj");

        var projects = solution.Projects
            .Where(p => p.Name.Contains("Infrastructure") && !p.Name.Contains("Shared"))
            .ToList();

        Console.WriteLine($"    !!! Drop database with command...!!!");
        await RunCommand(
            $"dotnet ef database drop --force --project \"{projects.FirstOrDefault().FilePath}\" --startup-project \"{startupProjectPath}\"");

        foreach (var project in projects)
        {
            Console.WriteLine($"Project: {project.Name}");

            var compilation = await project.GetCompilationAsync();
            var dbContexts = compilation?.SyntaxTrees
                .SelectMany(tree =>
                {
                    var semanticModel = compilation.GetSemanticModel(tree);
                    return tree.GetRoot().DescendantNodes()
                        .OfType<ClassDeclarationSyntax>()
                        .Where(cls =>
                        {
                            var symbol = semanticModel.GetDeclaredSymbol(cls);
                            return symbol?.BaseType?.Name == "DbContext";
                        });
                }).ToList();

            if (dbContexts == null || dbContexts.Count == 0)
            {
                Console.WriteLine($"  No DbContext was found in {project.Name}.");
                continue;
            }

            foreach (var ctx in dbContexts)
            {
                var ctxName = ctx.Identifier.Text;
                Console.WriteLine($"  \n\t------- Found DbContext: {ctxName}-------");

                var projectDir = Path.GetDirectoryName(project.FilePath)!;
                var migrationsDir = Path.Combine(projectDir, "Migrations");

                if (Directory.Exists(migrationsDir))
                {
                    Console.WriteLine("#############################################");
                    Console.WriteLine($"    Deleting of migration: {migrationsDir}");
                    Directory.Delete(migrationsDir, recursive: true);
                }
                else
                {
                    Console.WriteLine("Migrations not found!");
                }

                Console.WriteLine($"    Add migration...");
                await RunCommand(
                    $"dotnet ef migrations add Initial --project \"{project.FilePath}\" --startup-project \"{startupProjectPath}\" --context {ctxName}");

                Console.WriteLine($"    Update database...");
                await RunCommand(
                    $"dotnet ef database update --project \"{project.FilePath}\" --startup-project \"{startupProjectPath}\" --context {ctxName}");
            }
        }

        Console.WriteLine("Completed.");
    }
    private static async Task RunCommand(string command)
    {
        var psi = new System.Diagnostics.ProcessStartInfo("cmd", $"/c {command}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = System.Diagnostics.Process.Start(psi);
        if (process == null) return;

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        process.WaitForExit();

        if (!string.IsNullOrWhiteSpace(output))
            Console.WriteLine(output);

        if (!string.IsNullOrWhiteSpace(error))
            Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ResetColor();
    }


}