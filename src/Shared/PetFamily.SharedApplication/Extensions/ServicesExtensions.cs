﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions.CQRS;
using Scrutor;

namespace PetFamily.SharedApplication.Extensions;

public static class ServicesExtensions
{
    public static void CheckSectionsExistence(this IConfiguration configuration, List<string> sectionNames)
    {
        foreach (var sectionName in sectionNames)
        {
            var isSectionExists = configuration
                .GetSection(sectionName)
                .Exists();

            if (isSectionExists == false)
                throw new InvalidOperationException(
                    $"Configuration section '{sectionName}' is missing. " +
                    "Please ensure it is defined in your configuration file.");
        }
    }

    public static IServiceCollection AddCommandsAndQueries<TInjector>(this IServiceCollection services) =>
        services.Scan(scan => scan.FromAssemblies(typeof(TInjector).Assembly)
            .AddClasses(classes =>
                classes
                    .AssignableToAny(
                        typeof(ICommandHandler<,>),
                        typeof(ICommandHandler<>),
                        typeof(IQueryHandler<,>),
                        typeof(IQueryHandler<>)))
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime());
}
