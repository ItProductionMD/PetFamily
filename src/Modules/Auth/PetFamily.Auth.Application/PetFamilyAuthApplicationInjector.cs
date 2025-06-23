using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Auth.Application.AdminOptions;
using PetFamily.Auth.Application.DefaultSeeder;
using PetFamily.Auth.Application.Email;
using PetFamily.Auth.Application.IServices;
using PetFamily.SharedApplication.Extensions;
using static Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions;

namespace PetFamily.Auth.Application;

public static class PetFamilyAuthApplicationInjector
{
    public static IServiceCollection InjectPetFamilyAuthApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var isAdminSectionExists = configuration
            .GetSection(AdminIdentity.SectionName)
            .Exists();

        if (isAdminSectionExists == false)       
            throw new InvalidOperationException(
                $"Configuration section '{AdminIdentity.SectionName}' is missing. " +
                "Please ensure it is defined in your configuration file.");
        
        services.Configure<AdminIdentity>(configuration.GetSection(AdminIdentity.SectionName));

        services.AddCommandsAndQueries<ClassForAssemblyReference>();

        services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();

        services.AddTransient<RolesSeeder>();

        services.AddTransient<AdminSeeder>();

        return services;
    }
}
internal class ClassForAssemblyReference { }
