﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Infrastructure.Constants;
using PetFamily.Infrastructure.Contexts;
using PetFamily.Infrastructure.Contexts.ReadDbContext;
using PetFamily.Infrastructure.Repositories.Read;
using PetFamily.Infrastructure.Repositories.Write;
using PetFamily.Infrastructure.Services.BackgroundServices;
using PetFamily.Infrastructure.Services.MinioService;
//using Dapper;
using System.Data;
using Npgsql;
using PetFamily.Application.IRepositories;
using PetFamily.Application.Dtos;
using static PetFamily.Infrastructure.Dapper.Convertors;
using PetFamily.Infrastructure.Dapper;

namespace PetFamily.Infrastructure;

public static class Inject
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.GetConnectionString(ConnectionStringName.POSTGRESQL);

        services
            .AddScoped<IVolunteerReadRepository, VolunteerReadRepositoryWithDapper>()
            .AddScoped<IVolunteerWriteRepository, VolunteerWriteRepository>()
            .AddScoped<ISpeciesWriteRepository, SpeciesWriteRepository>()
            .AddScoped<ISpeciesReadRepository, SpeciesReadRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IFileRepository, MinioFileRepository>()
            .AddDbContext<ReadDbContext>(options => options.UseNpgsql(postgresConnection))
            .ConfigDapper(configuration)
            .AddScoped<IDbConnection>(sp => new NpgsqlConnection(postgresConnection))
            .AddScoped<WriteDbContext>()
            .AddHostedService<DbCleanupService>()
            .AddHostedService<MinioCleanupService>()
            .AddAWSClient(configuration)
            .AddMinio(configuration);


        return services;
    }
}
