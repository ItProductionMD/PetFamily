using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class ResetAndDropDatabaseV2
{
    public static void Run(string solutionRoot)
    {
        Console.WriteLine($"SolutionRooth:{solutionRoot}");
        var infrastructureProjects = Directory.GetDirectories(solutionRoot, "*Infrastructure*", SearchOption.AllDirectories)
            .Where(p => !p.Contains("Shared"));

        foreach (var projectPath in infrastructureProjects)
        {
            Console.WriteLine($"Scanning project: {projectPath}");
            var assemblyPath = Path.Combine(projectPath, "bin", "Debug", "net8.0"); // или net7.0
            if (!Directory.Exists(assemblyPath))
            {
                Console.WriteLine($"Build missing for {projectPath}, skipping.");
                continue;
            }

            var dllFiles = Directory.GetFiles(assemblyPath, "*.dll");
            foreach (var dll in dllFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    var dbContextTypes = assembly.GetTypes()
                        .Where(t => typeof(DbContext).IsAssignableFrom(t) && !t.IsAbstract)
                        .ToList();

                    foreach (var dbContextType in dbContextTypes)
                    {
                        Console.WriteLine($"Found DbContext: {dbContextType.FullName}");

                        var factoryType = assembly.GetTypes()
                            .FirstOrDefault(t => typeof(IDesignTimeDbContextFactory<>).MakeGenericType(dbContextType).IsAssignableFrom(t));

                        if (factoryType == null)
                        {
                            Console.WriteLine($"No factory found for {dbContextType.Name}, skipping.");
                            continue;
                        }

                        var factory = Activator.CreateInstance(factoryType);
                        var createMethod = factoryType.GetMethod("CreateDbContext", Type.EmptyTypes);
                        var dbContext = (DbContext)createMethod.Invoke(factory, null);

                        var services = new ServiceCollection();
                        var addDbContextFactoryMethod = typeof(EntityFrameworkServiceCollectionExtensions)
                            .GetMethods()
                            .First(m => m.Name == "AddDbContextFactory" && m.IsGenericMethod && m.GetParameters().Length == 2);

                        var genericMethod = addDbContextFactoryMethod.MakeGenericMethod(dbContextType);
                        genericMethod.Invoke(null, new object[] { services, null });
                        var provider = services.BuildServiceProvider();

                        //var migrationsAssembly = dbContext.GetService<IMigrationsAssembly>();
                        var migrationsAssembly = dbContextType.Assembly;
                        var migrationsPath = migrationsAssembly.Location;

                        var migrationsDirectory = Path.Combine(projectPath, "EFCore", "Migrations");
                        if (Directory.Exists(migrationsDirectory))
                            Directory.Delete(migrationsDirectory, true);

                        Console.WriteLine($"Add migration...");
                        var scaffolder = dbContext.GetInfrastructure().GetRequiredService<IMigrator>();


                        Console.WriteLine($"Update database...");
                        dbContext.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing assembly {dll}: {ex.Message}");
                }
            }
        }
    }
}

