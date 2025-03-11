using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.Species;
using PetFamily.Application.Volunteers;
using PetFamily.Infrastructure.Repositories;
using PetFamily.Infrastructure.Services.BackgroundServices;
using PetFamily.Infrastructure.Services.MinioService;

namespace PetFamily.Infrastructure;

public static class Inject
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddScoped<ISpeciesRepository, SpeciesRepository>()
            .AddScoped<IVolunteerRepository, VolunteerRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IFileRepository, MinioFileRepository>()
            .AddScoped<AppDbContext>()
            .AddHostedService<DbCleanupService>()
            .AddHostedService<MinioCleanupService>()
            .AddAWSClient(configuration)
            .AddMinio(configuration);
        return services;
    }
}
