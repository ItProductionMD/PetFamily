using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static PetFamily.Tools.DependencyProjectView.Extensions.GraphGeneratorExtensions;

namespace PetFamily.Tools.DependencyProjectView;

public partial class SolutionDependencyGraphGenerator
{
    private readonly string _mainDirectory;
    private readonly List<string> _excludeKeywords;
    private string _solutionName;
    private Dictionary<string, List<ProjectInfo>> Modules { get; set; } = [];
    public string OutputFilePath { get; set; } = "file isn't created";
    public SolutionDependencyGraphGenerator(
        string solutionDirectory,
        IEnumerable<string> excludeKeywords)
    {
        if (solutionDirectory == null)
            throw new ArgumentNullException($"Solution Directory is null!:{nameof(solutionDirectory)}");

        _mainDirectory = Directory.GetParent(solutionDirectory)!.FullName;

        Console.WriteLine("########################################");
        Console.WriteLine($"\tMain directory:{_mainDirectory}");
        Console.WriteLine("########################################");

        _solutionName = Path.GetFileNameWithoutExtension(solutionDirectory);

        Console.WriteLine("########################################");
        Console.WriteLine($"\t_solutionName:{_solutionName}");
        Console.WriteLine("########################################");

        SetFilePath();

        _excludeKeywords = excludeKeywords?.ToList() ?? new List<string>();
    }

    public void GenerateDependencyGraph()
    {
        var csprojFilesPaths = Directory
            .GetFiles(_mainDirectory, "*.csproj", SearchOption.AllDirectories)
            .Where(path => path.ContainsKeyWord(_excludeKeywords) == false)
            .ToList();
        Console.WriteLine("########################################");
        Console.WriteLine($"\tProjectPaths count:{csprojFilesPaths.Count}");
        Console.WriteLine("########################################");

        foreach (var csprojFilePath in csprojFilesPaths)
        {
            Console.WriteLine($"\tOpen proj:{csprojFilePath}");
            Console.WriteLine("#######################################");

            var doc = XDocument.Load(csprojFilePath);

            var projectInfo = ProjectInfo.Create(
                _solutionName,
                csprojFilePath,
                doc,
                _excludeKeywords);

            if (Modules.ContainsKey(projectInfo.ModuleName) == false)
                Modules[projectInfo.ModuleName] = [projectInfo];

            else
                Modules[projectInfo.ModuleName].Add(projectInfo);
        }
        GenerateDotFile();
    }

    private void GenerateDotFile()
    {
        using (var writer = new StreamWriter(OutputFilePath))
        {
            writer.WriteLine("digraph solution_dependencies {");
            writer.WriteLine("rankdir=TD;" +
                "\r\n    graph[" +
                "\r\n   fontname=\"Helvetica\"," +
                "\r\n  fontsize=14, " +
                "\r\n \r\n  overlap=false," +
                "\r\n  nodesep=0.7," +
                " \r\n  ranksep=1.0];" +
                "\r\n  node [\r\n     " +
                " shape=box, \r\n   " +
                "   style=filled, \r\n  " +
                "    fillcolor=lightblue,\r\n " +
                "     fontname=\"Helvetica\",\r\n " +
                "     fontsize=18,\r\n   " +
                "   width=3.0,\r\n    " +
                "  height=1.0,\r\n  " +
                "    fixedsize=true];");

            // Добавляем узлы только для включенных проектов
            foreach (var module in Modules)
            {
                WriteSubGraph(module, writer);
            }
            foreach (var module in Modules)
            {
                foreach (var project in module.Value)
                {
                    WriteReferences(project, writer);
                }
            }
            writer.WriteLine("}");
        }
        Console.WriteLine($"###File path :{OutputFilePath}");
        OpenInGraphvizOnline(OutputFilePath);
    }

    private void SetFilePath()
    {
        string outputDirectory = Path.Combine(_mainDirectory, "diagrams");
        Directory.CreateDirectory(outputDirectory);

        OutputFilePath = Path.Combine(outputDirectory, $"{Guid.NewGuid()}_diagram.dot");
        Console.WriteLine($"###OutPutFilePath set to:{OutputFilePath}###");
    }

    private void WriteSubGraph(
        KeyValuePair<string, List<ProjectInfo>> keyValue,
        StreamWriter writer)
    {
        var moduleName = keyValue.Key;
        var projects = keyValue.Value;
        writer.WriteLine($"    subgraph cluster_{moduleName} {{");
        writer.WriteLine($"        label = \"{moduleName}\";");
        writer.WriteLine("        style=filled;");
        writer.WriteLine("        fillcolor=lightblue;");
        writer.WriteLine("        fontname=\"Helvetica\";");
        writer.WriteLine("        color = lightblue;");
        writer.WriteLine("        {");
        if(projects.FirstOrDefault().ModuleName.ContainsKeyWord(["Public","Contract"]))
            writer.WriteLine("             rank=same;");
        foreach (var projectInfo in projects)
        {

            writer.WriteLine($"        \"{projectInfo.Name}\" [style=filled, fillcolor=\"{projectInfo.Color}\", fontcolor=\"white\", shape=box];");
        }
        writer.WriteLine("         }");
        writer.WriteLine("    }");
    }

    private void WriteReferences(
        ProjectInfo project,
        StreamWriter writer)
    {
        foreach (var reference in project.ProjectReferences)
        {
            writer.WriteLine($"    \"{project.Name}\" -> \"{reference}\" [color=\"{project.Color}\"];");
        }
    }

    private void OpenInGraphvizOnline(string dotFilePath)
    {
        string dotContent = File.ReadAllText(dotFilePath);
        string encodedContent = Uri.EscapeDataString(dotContent);

        string url = $"https://dreampuf.github.io/GraphvizOnline/#{encodedContent}";

        // Открываем URL в браузере
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}
