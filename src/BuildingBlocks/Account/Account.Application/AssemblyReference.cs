using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Account.Application.DefaultSeeder;
using Account.Application.Options;
using PetFamily.SharedApplication.Extensions;

using static Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions;

namespace Account.Application;

public static class AccountCommandsAndQueriesInjector
{
    public static IServiceCollection InjectCommandsAndQueries(this IServiceCollection services)
    {
        services.AddCommandsAndQueries<UserAccountApplicationForAssemblyReference>();
        return services;
    }
}

internal class UserAccountApplicationForAssemblyReference { }
