using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedApplication.DependencyInjection;

namespace PetSpecies.Application;

public static class SpeciesApplicationInjector
{
    public static IServiceCollection InjectSpeciesApplication(
        this IServiceCollection services,
        IConfiguration configuration) =>

    services.AddCommandsAndQueries<ClassForAssemblyReference>();

    //.AddValidatorsFromAssembly(typeof(SpeciesApplicationInjector).Assembly)

    internal class ClassForAssemblyReference
    {
        // This class is used to reference the assembly containing the validators.
        // It ensures that the assembly is loaded and the validators are registered.
    }
}
