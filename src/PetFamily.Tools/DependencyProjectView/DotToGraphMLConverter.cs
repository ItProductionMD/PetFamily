using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace PetFamily.Tools.DependencyProjectView;
public static class DotToGraphMLConverter
{
    public static void ConvertDotToGraphML(string dotFilePath, string graphmlOutputPath)
    {
        var lines = File.ReadAllLines(dotFilePath);

        // Парсим узлы: "Имя" [label="..."];
        var nodePattern = new Regex(@"^\s*""([^""]+)""\s*\[label=""([^""]+)""\];");
        // Парсим рёбра: "От" -> "К";
        var edgePattern = new Regex(@"^\s*""([^""]+)""\s*->\s*""([^""]+)"";");

        var nodes = new Dictionary<string, string>(); // id -> label
        var edges = new List<(string from, string to)>();

        foreach (var line in lines)
        {
            var nodeMatch = nodePattern.Match(line);
            if (nodeMatch.Success)
            {
                var id = EscapeXmlId(nodeMatch.Groups[1].Value);
                var label = nodeMatch.Groups[2].Value.Replace("\\n", "\n");
                nodes[id] = label;
                continue;
            }

            var edgeMatch = edgePattern.Match(line);
            if (edgeMatch.Success)
            {
                var from = EscapeXmlId(edgeMatch.Groups[1].Value);
                var to = EscapeXmlId(edgeMatch.Groups[2].Value);
                edges.Add((from, to));
            }
        }

        using var writer = new StreamWriter(graphmlOutputPath);

        writer.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        writer.WriteLine(@"<graphml xmlns=""http://graphml.graphdrawing.org/xmlns"" ");
        writer.WriteLine(@"         xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" ");
        writer.WriteLine(@"         xsi:schemaLocation=""http://graphml.graphdrawing.org/xmlns ");
        writer.WriteLine(@"         http://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd"">");
        writer.WriteLine(@"  <graph id=""G"" edgedefault=""directed"">");

        // Пишем узлы
        foreach (var (id, label) in nodes)
        {
            writer.WriteLine($@"    <node id=""{id}"">");
            writer.WriteLine($@"      <data key=""label"">{System.Security.SecurityElement.Escape(label)}</data>");
            writer.WriteLine(@"    </node>");
        }

        // Пишем рёбра
        int edgeId = 0;
        foreach (var (from, to) in edges.Distinct())
        {
            writer.WriteLine($@"    <edge id=""e{edgeId++}"" source=""{from}"" target=""{to}"" />");
        }

        writer.WriteLine(@"  </graph>");
        writer.WriteLine(@"</graphml>");
    }

    private static string EscapeXmlId(string id)
    {
        // Убираем пробелы и недопустимые символы
        return id.Replace(' ', '_').Replace('.', '_').Replace('-', '_');
    }
}

