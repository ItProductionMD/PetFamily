using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedApplication.DependencyInjection;
using PetFamily.VolunteerRequests.Application.ContractsImplementation;
using PetFamily.VolunteerRequests.Public.Contracts;

namespace PetFamily.VolunteerRequests.Application;

public static class VolunteerRequestApplicationInjector
{
    public static IServiceCollection InjectVolunteerRequestApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddCommandsAndQueries<ClassForAssemblyReference>()
            .AddScoped<IVolunteerRequestSubmission, VolunteerRequestSubmission>();

        return services;
    }
}

internal class ClassForAssemblyReference { }

