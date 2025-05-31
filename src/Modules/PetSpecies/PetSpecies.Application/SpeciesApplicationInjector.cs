using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetSpecies.Application.Queries.GetSpeciesPagedList;
using FluentValidation;
using Scrutor;
using PetFamily.Application.Abstractions.CQRS;


namespace PetSpecies.Application;

public static class SpeciesApplicationInjector
{
    public static IServiceCollection InjectSpeciesApplication(
        this IServiceCollection services,
        IConfiguration configuration) =>

    services.AddCommandsAndQueries();

    //.AddValidatorsFromAssembly(typeof(SpeciesApplicationInjector).Assembly)

    private static IServiceCollection AddCommandsAndQueries(this IServiceCollection services) =>

        services.Scan(scan => scan.FromAssemblies(typeof(SpeciesApplicationInjector).Assembly)
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
