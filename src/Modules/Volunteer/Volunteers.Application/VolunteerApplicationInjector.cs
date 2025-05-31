using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions.CQRS;

namespace Volunteers.Application;

public static class VolunteerApplicationInjector
{
    public static IServiceCollection InjectVolunteerApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(VolunteerApplicationInjector).Assembly);
        services.AddCommandsAndQueries();
        services.AddSingleton<PetImagesValidatorOptions>();

        return services;
    }



    private static IServiceCollection AddCommandsAndQueries(this IServiceCollection services)
    {
        return services.Scan(scan => scan.FromAssemblies(typeof(VolunteerApplicationInjector).Assembly)
                    .AddClasses(classes =>
                        classes.AssignableToAny(
                            typeof(ICommandHandler<,>),
                            typeof(ICommandHandler<>),
                            typeof(IQueryHandler<,>),
                            typeof(IQueryHandler<>)))
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime());
    }
}
