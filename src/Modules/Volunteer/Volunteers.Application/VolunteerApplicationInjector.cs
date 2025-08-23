using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Account.Public.Contracts;
using Volunteers.Application.Contracts;
using Volunteers.Public.IContracts;
using PetFamily.SharedApplication.DependencyInjection;

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
        services.AddScoped<IVolunteerPhoneUpdater, VolunteerPhoneUpdater>();

        return services;
    }

    internal class ClassForAssemblyReference { }
}
