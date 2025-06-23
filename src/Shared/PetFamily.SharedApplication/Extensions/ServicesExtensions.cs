using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions.CQRS;
using Scrutor;
namespace PetFamily.SharedApplication.Extensions;

public static class ServicesExtensions
{
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
