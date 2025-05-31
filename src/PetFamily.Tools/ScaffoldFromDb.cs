using Dapper;
using Npgsql;
using System.Text;
using System.Text.RegularExpressions;

namespace PetFamily.Tools;
public class ScaffoldFromDb
{
    private readonly string _connectionString;

    public ScaffoldFromDb(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Run(string infrastructurePath)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var query = @"
            SELECT table_name, column_name 
            FROM information_schema.columns 
            WHERE table_schema = 'public'
            ORDER BY table_name, ordinal_position";

        var columns = connection.Query<(string TableName, string ColumnName)>(query);
        var tables = columns.GroupBy(c => c.TableName);

        string outputPath = Path.Combine(infrastructurePath, "Dapper", "GeneratedTables");
        if (Directory.Exists(outputPath) == false!)
        {
            Console.WriteLine($"Directory :{outputPath} not exist!Make shure you set command from write directory! ");
            return;
        }

        foreach (var table in tables)
        {
            string className = ToPascalCase(table.Key);
            string filePath = Path.Combine(outputPath, className + ".cs");

            var newProperties = table.Select(c => (Name: ToPascalCase(c.ColumnName), Value: c.ColumnName)).ToList();

            if (File.Exists(filePath))
            {
                var existingProperties = ParseExistingClass(filePath);
                var added = newProperties.Except(existingProperties).ToList();
                var removed = existingProperties.Except(newProperties).ToList();

                if (added.Any() || removed.Any())
                {
                    Console.WriteLine($"Changed in {className}:");
                    if (added.Any()) Console.WriteLine(" + Added: " + string.Join(", ", added.Select(p => p.Name)));
                    if (removed.Any()) Console.WriteLine(" - Deleted: " + string.Join(", ", removed.Select(p => p.Name)));

                    File.WriteAllText(filePath, GenerateClassContent(className, table.Key, newProperties));
                }
            }
            else
            {
                Console.WriteLine($"Created new class: {className}");
                File.WriteAllText(filePath, GenerateClassContent(className, table.Key, newProperties));
            }
        }

        Console.WriteLine("Generation was completed!");
    }

    private string GenerateClassContent(string className, string tableName, List<(string Name, string Value)> properties)
    {
        var sb = new StringBuilder();
        sb.AppendLine("public static class " + className);
        sb.AppendLine("{");
        sb.AppendLine($"    public const string Table = \"{tableName}\";");
        foreach (var prop in properties)
            sb.AppendLine($"    public const string {prop.Name} = \"{prop.Value}\";");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private List<(string Name, string Value)> ParseExistingClass(string filePath)
    {
        var properties = new List<(string Name, string Value)>();
        var lines = File.ReadAllLines(filePath);
        var regex = new Regex(@"public const string (\w+) = ""(.*?)"";");

        foreach (var line in lines)
        {
            var match = regex.Match(line);
            if (match.Success)
                properties.Add((match.Groups[1].Value, match.Groups[2].Value));
        }

        return properties;
    }

    private string ToPascalCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("input is null!");
            throw new ArgumentNullException("input");
        }
        var strings = input.Split('_');
        string result = string.Empty;
        foreach (var item in strings)
        {
            if (!string.IsNullOrEmpty(item) && char.IsLetter(item[0]))
            {
                var word = item.ToLower();
                var pascaleCaseWord = char.ToUpper(word[0]) + word.Substring(1);
                result = result + pascaleCaseWord;
            }
            else
            {
                result = result + item;
            }
        }
        return result;
    }
}

