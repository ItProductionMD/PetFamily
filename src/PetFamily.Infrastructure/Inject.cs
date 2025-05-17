using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Infrastructure.Constants;
using PetFamily.Infrastructure.Contexts;
using PetFamily.Infrastructure.Contexts.ReadDbContext;
using PetFamily.Infrastructure.Dapper;
using PetFamily.Infrastructure.Repositories.Read;
using PetFamily.Infrastructure.Repositories.Write;
using PetFamily.Infrastructure.Services.BackgroundServices;
using PetFamily.Infrastructure.Services.MinioService;

namespace PetFamily.Infrastructure;

public static class Inject
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.GetConnectionString(ConnectionStringName.POSTGRESQL);
        if (string.IsNullOrEmpty(postgresConnection))
            throw new ApplicationException("PostgreSQL connection string wasn't found!");

        services
            .AddScoped<IVolunteerReadRepository, VolunteerReadRepositoryWithDapper>()
            .AddScoped<IVolunteerWriteRepository, VolunteerWriteRepository>()
            .AddScoped<ISpeciesWriteRepository, SpeciesWriteRepository>()
            .AddScoped<ISpeciesReadRepository, SpeciesReadRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IFileRepository, MinioFileRepository>()
            .AddDbContext<ReadDbContext>(options => options.UseNpgsql(postgresConnection))
            .ConfigDapper(configuration)
            .AddSingleton<IDbConnectionFactory>(_ => new NpgSqlConnectionFactory(postgresConnection))
            .AddScoped<WriteDbContext>(_ => new WriteDbContext(postgresConnection))
            .AddHostedService<DbCleanupService>()
            .AddHostedService<MinioCleanupService>()
            .AddAWSClient(configuration)
            .AddMinio(configuration);


        return services;
    }
}
