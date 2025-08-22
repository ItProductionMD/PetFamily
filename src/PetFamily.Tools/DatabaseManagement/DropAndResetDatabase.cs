using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System.Diagnostics;

namespace PetFamily.Tools.DatabaseManagement;

public static class DropAndResetDatabase
{
    public static async Task RunAsync(string solutionPath)
    {
        Console.WriteLine("Run DropAndResetDatabase...");

        using var workspace = MSBuildWorkspace.Create();
        Console.WriteLine("workspace created");

        var solution = await workspace.OpenSolutionAsync(solutionPath);
        Console.WriteLine($"solution opened! Path: {solution.FilePath}");

        var startupProjectPath = Path.Combine(
            Path.GetDirectoryName(solutionPath)!,
            "src", "PetFamily.Host.Api", "PetFamily.Host.Api.csproj");

        var projects = solution.Projects
            .Where(p => p.Name.Contains("Infrastructure") && !p.Name.Contains("Shared"))
            .ToList();

        var tasks = projects.Select(async project =>
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
                Console.WriteLine($"  No DbContext found in {project.Name}.");
                return;
            }

            foreach (var ctx in dbContexts)
            {
                var ctxName = ctx.Identifier.Text;
                Console.WriteLine($"  Found DbContext: {ctxName}");

                var projectDir = Path.GetDirectoryName(project.FilePath)!;
                var migrationsDir = Path.Combine(projectDir, "Migrations");

                if (Directory.Exists(migrationsDir))
                {
                    Console.WriteLine($"    Deleting migrations in {migrationsDir}");
                    Directory.Delete(migrationsDir, recursive: true);
                }
                else
                {
                    Console.WriteLine("    Migrations directory not found.");
                }

                await RunCommandAsync($"dotnet ef database drop --force --project \"{project.FilePath}\" --startup-project \"{startupProjectPath}\"");
                await RunCommandAsync($"dotnet ef migrations add Initial --project \"{project.FilePath}\" --startup-project \"{startupProjectPath}\" --context {ctxName}");
                await RunCommandAsync($"dotnet ef database update --project \"{project.FilePath}\" --startup-project \"{startupProjectPath}\" --context {ctxName}");
            }
        });

        await Task.WhenAll(tasks);
        Console.WriteLine("Completed.");
    }

    private static async Task RunCommandAsync(string command)
    {
        var psi = new ProcessStartInfo("cmd", $"/c {command}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null) return;

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await Task.WhenAll(outputTask, errorTask);
        await process.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(outputTask.Result))
            Console.WriteLine(outputTask.Result);

        if (!string.IsNullOrWhiteSpace(errorTask.Result))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorTask.Result);
            Console.ResetColor();
        }
    }
}

