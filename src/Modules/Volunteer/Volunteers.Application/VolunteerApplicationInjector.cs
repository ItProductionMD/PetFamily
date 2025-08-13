using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Auth.Public.Contracts;
using PetFamily.SharedApplication.Extensions;
using Volunteers.Application.Contracts;
using Volunteers.Public.IContracts;

namespace Volunteers.Application;

public static class VolunteerApplicationInjector
{
    public static IServiceCollection InjectVolunteerApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IVolunteerCreator, VolunteerCreator>();
        services.AddScoped<IParticipantContract, ParticipantContract>();
        services.AddValidatorsFromAssembly(typeof(VolunteerApplicationInjector).Assembly);
        services.AddCommandsAndQueries<ClassForAssemblyReference>();
        services.AddSingleton<PetImagesValidatorOptions>();

        return services;
    }

    internal class ClassForAssemblyReference { }
}
