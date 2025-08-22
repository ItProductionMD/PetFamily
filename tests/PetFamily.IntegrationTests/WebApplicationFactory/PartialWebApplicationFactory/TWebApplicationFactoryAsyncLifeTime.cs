using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PetFamily.SharedInfrastructure.Constants;
using Respawn;

namespace PetFamily.IntegrationTests.WebApplicationFactory;

public partial class TestWebApplicationFactory
{
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());

        ConnectionString = _dbContainer.GetConnectionString();

        await ApplyAllMigrationsToDbContainerAsync();

        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
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

    public async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,

            SchemasToInclude = new[]
            {
                SchemaNames.SPECIES,
                SchemaNames.VOLUNTEER,
                SchemaNames.USER_ACCOUNT,
                SchemaNames.VOLUNTEER_REQUEST,
                SchemaNames.DISCUSSION
            }

        });
    }

    public async Task ResetCheckpoint()
    {
        Console.WriteLine("ResetBd");
        await _respawner.ResetAsync(_dbConnection);
    }

}
