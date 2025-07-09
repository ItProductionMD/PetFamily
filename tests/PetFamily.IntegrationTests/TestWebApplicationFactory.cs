using FileStorage.Application.IRepository;
using FileStorage.Public.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Npgsql;
using PetFamily.Application.Abstractions;
using PetFamily.SharedInfrastructure.Shared.Dapper;
using PetSpecies.Infrastructure.Contexts;
using Respawn;
using Testcontainers.PostgreSql;
using Volunteers.Infrastructure.Contexts;
using PetFamily.SharedInfrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using PetFamily.Auth.Infrastructure.Contexts;

namespace PetFamily.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("testpetfamilydb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private Respawner _respawner = null!;
    private NpgsqlConnection _dbConnection = null!;
    public Mock<IFileService> FileServiceMock = new Mock<IFileService>();
    public Mock<IUploadFileDtoValidator> FileValidator = new Mock<IUploadFileDtoValidator>();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var speciesWriteDbContext = services.SingleOrDefault(s => s.ServiceType == typeof(SpeciesWriteDbContext));
            if (speciesWriteDbContext != null)
                services.Remove(speciesWriteDbContext);

            var volunteerWriteDbContext = services.SingleOrDefault(s => s.ServiceType == typeof(VolunteerWriteDbContext));
            if (volunteerWriteDbContext != null)
                services.Remove(volunteerWriteDbContext);

            var authWriteDbContext = services.SingleOrDefault(s => s.ServiceType == typeof(AuthWriteDbContext));
            if (authWriteDbContext != null)
                services.Remove(authWriteDbContext);

            var iDbConnection = services.SingleOrDefault(s => s.ServiceType == typeof(IDbConnectionFactory));
            if (iDbConnection != null)
                services.Remove(iDbConnection);

            var iFileService = services.SingleOrDefault(s => s.ServiceType == typeof(IFileService));
            if (iFileService != null)
                services.Remove(iFileService);

            services.AddScoped<SpeciesWriteDbContext>(_ =>
                new SpeciesWriteDbContext(_dbContainer.GetConnectionString()));

            services.AddScoped<VolunteerWriteDbContext>(_ =>
                new VolunteerWriteDbContext(_dbContainer.GetConnectionString()));

            services.AddScoped<AuthWriteDbContext>(_ =>
                new AuthWriteDbContext(_dbContainer.GetConnectionString()));

            services.AddSingleton<IDbConnectionFactory>(_ =>
                new NpgSqlConnectionFactory(_dbContainer.GetConnectionString()));

            services.AddScoped<IFileService>(_ => FileServiceMock.Object);
        });
    }

    public async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,

            SchemasToInclude = new[] { SchemaNames.SPECIES, SchemaNames.VOLUNTEER , SchemaNames.AUTH }

        });
    }

    public async Task ResetCheckpoint()
    {
        Console.WriteLine("ResetBd");
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        //create Tables
        using var scope = Services.CreateScope();
        var speciesContext = scope.ServiceProvider.GetRequiredService<SpeciesWriteDbContext>();
        var volunteerContext = scope.ServiceProvider.GetRequiredService<VolunteerWriteDbContext>();
        var authContext = scope.ServiceProvider.GetRequiredService<AuthWriteDbContext>();

        await authContext.Database.MigrateAsync();
        await speciesContext.Database.MigrateAsync();
        await volunteerContext.Database.MigrateAsync();


        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}
