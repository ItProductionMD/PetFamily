using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Auth.Application.AdminOptions;
using PetFamily.Auth.Application.DefaultSeeder;
using PetFamily.Auth.Application.Email;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Application.Options;
using PetFamily.SharedApplication.Extensions;
using static Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions;

namespace PetFamily.Auth.Application;

public static class PetFamilyAuthApplicationInjector
{
    public static IServiceCollection InjectPetFamilyAuthApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        configuration.CheckSectionsExistence([
            AdminIdentity.SECTION_NAME, 
            RefreshTokenCookie.SECTION_NAME]);
        
        services.Configure<AdminIdentity>(configuration.GetSection(AdminIdentity.SECTION_NAME));

        services.Configure<RefreshTokenCookie>(configuration.GetSection(RefreshTokenCookie.SECTION_NAME));

        services.AddCommandsAndQueries<ClassForAssemblyReference>();

        services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();

        services.AddTransient<RolesSeeder>();

        services.AddTransient<AdminSeeder>();

        return services;
    }

   
}
internal class ClassForAssemblyReference { }
