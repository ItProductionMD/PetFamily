using Microsoft.Extensions.Configuration;
using PetFamily.Infrastructure.Dapper;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Using: dotnet run -- [command]");
            Console.WriteLine("list of commands: scaffold");
            return;
        }

        switch (args[0].ToLower())
        {
            case "scaffold":
                RunScaffold();
                break;
            default:
                Console.WriteLine($"unknown command: {args[0]}");
                break;
        }
    }
    static string GetConnectionString()
    {
        string pathToDll = Path.Combine(
           Directory.GetParent(Directory.GetCurrentDirectory())!.FullName,  
           "PetFamily.API",  
           "bin", "Debug", "net9.0", "PetFamily.API.dll"  
       );

        if (!File.Exists(pathToDll))
            throw new FileNotFoundException($"Assemby fail not found: {pathToDll}");

        var mainAssembly = Assembly.LoadFile(pathToDll);

        var config = new ConfigurationBuilder()
            .AddUserSecrets(mainAssembly)
            .Build();

        return config.GetConnectionString("PostgreForPetFamily") 
            ?? throw new Exception("Connection string not found.");
    }
    /// <summary>
    /// Generate classes from DBTable with column names to use Dapper easy
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    private static void RunScaffold()
    {
        string pathToInfrastructure = Path.Combine(
          Directory.GetParent(Directory.GetCurrentDirectory())!.FullName,
          "PetFamily.Infrastructure"
        );

        if (!Path.Exists(pathToInfrastructure))
            throw new FileNotFoundException($"Assemby fail not found: {pathToInfrastructure}");
        string connectionString = GetConnectionString();
        Console.WriteLine($"Connection String: {connectionString}");

        var scaffolder = new ScaffoldFromDb(connectionString);
        scaffolder.Run(pathToInfrastructure);
    }
}

