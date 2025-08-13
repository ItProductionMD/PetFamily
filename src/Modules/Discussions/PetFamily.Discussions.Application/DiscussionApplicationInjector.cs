using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedApplication.Extensions;

namespace PetFamily.Discussions.Application;

public static class DiscussionApplicationInjector
{
    public static IServiceCollection InjectDiscussionApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCommandsAndQueries<ClassForAssemblyReference>();

        return services;
    }

    internal class ClassForAssemblyReference { }
}
