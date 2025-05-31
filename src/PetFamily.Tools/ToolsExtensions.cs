using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace PetFamily.Tools;

public static class ToolsExtensions
{
    /// <summary>
    /// Get connection string from appsettings.json
    /// TODO ADD: add to appsettings.json automatically
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public static string GetConnectionString()
    {
        string pathToDll = Path.Combine(
           Directory.GetParent(Directory.GetCurrentDirectory())!.FullName,
           "PetFamily.Host.Api",
           "bin", "Debug", "net9.0", "PetFamily.Host.Api.dll"
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
}
