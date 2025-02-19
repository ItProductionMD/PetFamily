﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.Pets;
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
            .AddScoped<IPetRepository,PetRepository>()
            .AddScoped<IVolunteerRepository, VolunteerRepository>()
            .AddScoped<IFileRepository, MinioFileRepository>()
            .AddScoped<AppDbContext>()
            .AddHostedService<CleanupService>()
            .AddMinio(configuration);
        return services;
    }
}
