using Azure;
using Bogus.DataSets;
using Docker.DotNet.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Npgsql;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Enums;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Infrastructure.Contexts;
using Polly;
using Respawn;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Testcontainers.PostgreSql;
using Address = PetFamily.Domain.Shared.ValueObjects.Address;

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

    private Respawner _respawner =null!;
    private NpgsqlConnection _dbConnection = null!;
    public Mock<IFileRepository> FileServiceMock = new Mock<IFileRepository>();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var writeDbContext = services.SingleOrDefault(s => s.ServiceType == typeof(WriteDbContext));
            if (writeDbContext != null)
                services.Remove(writeDbContext);

            var iDbConnection = services.SingleOrDefault(s => s.ServiceType == typeof(IDbConnection));
            if (iDbConnection != null)
                services.Remove(iDbConnection);

            var iFileService = services.SingleOrDefault(s => s.ServiceType == typeof(IFileRepository));
            if (iFileService != null)
                services.Remove(iFileService);

            services.AddScoped<WriteDbContext>(_ =>
                new WriteDbContext(_dbContainer.GetConnectionString()));

            services.AddScoped<IDbConnection>(_ =>
                new NpgsqlConnection(_dbContainer.GetConnectionString()));

            services.AddScoped<IFileRepository>(_ => FileServiceMock.Object);
        });
    }
   
    public async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
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
        var context = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        await context.Database.EnsureCreatedAsync();
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}
