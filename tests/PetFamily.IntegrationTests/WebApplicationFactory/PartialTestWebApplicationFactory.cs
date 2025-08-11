using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PetFamily.IntegrationTests.WebApplicationFactory.Extensions;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedInfrastructure.Shared.Dapper;
namespace PetFamily.IntegrationTests.WebApplicationFactory;

public partial class TestWebApplicationFactory
{
    private List<Type> _writeDbContextTypes = new();
  
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            _writeDbContextTypes =  services.ReplaceAllWriteDbContexts(ConnectionString);

            services.AddSingleton<IDbConnectionFactory>(_ => new NpgSqlConnectionFactory(ConnectionString));

            services.AddScoped(_ => ParticipantContractMock.Object);
            services.AddScoped(_ => FileServiceMock.Object);

            //TODO DELETE FROM TESTS AND APPLICATION LAYER  AND MOVE IT IN PRESENTATION LAYER
            services.AddScoped(_ => UserContextMock.Object);
        });
    }
    private void ApplyAllMigrationsToDbContainer()
    {
        using var scope = Services.CreateScope();

        foreach(var type in _writeDbContextTypes)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService(type) as DbContext;
            if (dbContext == null)
            {
                throw new InvalidOperationException($"Service of type {type.Name} is not a DbContext.");
            }
            dbContext.Database.Migrate();
        }
    }
    private async Task ApplyAllMigrationsToDbContainerAsync()
    {
        using var scope = Services.CreateScope();

        int total = _writeDbContextTypes.Count;
        int current = 0;

        foreach (var type in _writeDbContextTypes)
        {
            current++;
            Console.WriteLine($"[{current}/{total}] Applying migrations for {type.Name}...");

            try
            {
                if (scope.ServiceProvider.GetRequiredService(type) is DbContext dbContext)
                {
                    await dbContext.Database.MigrateAsync();
                    Console.WriteLine($"[{current}/{total}] Migrations applied for {type.Name}");
                }
                else
                {
                    throw new InvalidOperationException($"Service of type {type.Name} is not a DbContext.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error migrating {type.Name}: {ex.Message}");
                throw;
            }
        }

        Console.WriteLine("All migrations applied successfully.");
    }


}
