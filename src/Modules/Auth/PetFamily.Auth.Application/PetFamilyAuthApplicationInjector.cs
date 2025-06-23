using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Auth.Application.DefaultSeeder;
using PetFamily.Auth.Application.Email;
using PetFamily.Auth.Application.IServices;
using PetFamily.SharedApplication.Extensions;

namespace PetFamily.Auth.Application;

public static class PetFamilyAuthApplicationInjector
{
    public static IServiceCollection InjectPetFamilyAuthApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCommandsAndQueries<ClassForAssemblyReference>();

        services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();

        services.AddTransient<RolesSeeder>();

        return services;
    }
}
internal class ClassForAssemblyReference { }
