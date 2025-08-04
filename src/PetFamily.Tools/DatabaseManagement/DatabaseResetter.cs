using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PetFamily.Tools.DatabaseManagement;

public class DatabaseResetter
{
    private readonly string _solutionPath;
    private readonly string _startupProjectPath;
    private readonly MSBuildWorkspace _workspace;

    public DatabaseResetter(string solutionPath)
    {
        _solutionPath = solutionPath;
        _startupProjectPath = Path.Combine(
            Path.GetDirectoryName(solutionPath)!,
            "src", "PetFamily.Host.Api", "PetFamily.Host.Api.csproj");
        _workspace = MSBuildWorkspace.Create();
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Run DropAndResetDatabase...");
        Console.WriteLine("workspace created");

        var solution = await _workspace.OpenSolutionAsync(_solutionPath);
        Console.WriteLine($"solution opened!Path:{solution.FilePath}");

        var projects = solution.Projects
            .Where(p => p.Name.Contains("Infrastructure") && !p.Name.Contains("Shared"))
            .ToList();

        var tasks = projects.Select(ResetProjectAsync);
        await Task.WhenAll(tasks);

        Console.WriteLine("Completed.");
    }

    private async Task ResetProjectAsync(Project project)
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
                Console.WriteLine($"    Deleting of migration: {migrationsDir}");
                Directory.Delete(migrationsDir, recursive: true);
            }
            else
            {
                Console.WriteLine("Migrations not found!");
            }

            Console.WriteLine($"    Drop database with command...");
            await RunCommandAsync(
                $"dotnet ef database drop --force --project \"{project.FilePath}\" --startup-project \"{_startupProjectPath}\"");

            Console.WriteLine($"    Add migration...");
            await RunCommandAsync(
                $"dotnet ef migrations add Initial --project \"{project.FilePath}\" --startup-project \"{_startupProjectPath}\" --context {ctxName}");

            Console.WriteLine($"    Update database...");
            await RunCommandAsync(
                $"dotnet ef database update --project \"{project.FilePath}\" --startup-project \"{_startupProjectPath}\" --context {ctxName}");
        }
    }

    private async Task RunCommandAsync(string command)
    {
        var psi = new ProcessStartInfo("cmd", $"/c {command}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(psi);
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

