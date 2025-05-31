//using PetFamily.Infrastructure.Dapper;
using static PetFamily.Tools.ToolsExtensions;

namespace PetFamily.Tools;

public static class Scaffolder
{
    /// <summary>  
    /// Generate classes from DBTable with column names to use Dapper easily  
    /// TODO automatically add namespaces and using to generated classes  
    /// </summary>  
    /// <exception cref="FileNotFoundException"></exception>  
    public static void RunScaffold()
    {
        string pathToInfrastructure = Path.Combine(
          Directory.GetParent(Directory.GetCurrentDirectory())!.FullName,
          "PetFamily.Infrastructure"
        );
        if (!Path.Exists(pathToInfrastructure))
            throw new FileNotFoundException($"Assembly fail not found: {pathToInfrastructure}");

        string connectionString = GetConnectionString();
        Console.WriteLine($"Connection String: {connectionString}");

        var scaffolder = new ScaffoldFromDb(connectionString);
        scaffolder.Run(pathToInfrastructure);
    }
}
