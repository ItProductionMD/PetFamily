using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PetFamily.SharedApplication.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace PetFamily.SharedApplication.Extensions;

public static class OptionsExtensions
{
    public static IServiceCollection AddCustomOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        OptionsLifetime lifetime = OptionsLifetime.Singleton)
        where TOptions : class, IApplicationOptions, new()
    {
        var sectionName = TOptions.GetSectionName();

        var section = configuration.GetSection(sectionName);

        switch (lifetime)
        {
            case OptionsLifetime.Singleton:
                {
                    var options = section.Get<TOptions>();

                    if (!options.ValidateOptions(sectionName))
                    {
                        throw new OptionsValidationException(
                            sectionName,
                            typeof(TOptions),
                            new[] { $"Configuration section '{sectionName}' is missing or invalid." });
                    }

                    services.AddSingleton(options!);
                    break;
                }

            case OptionsLifetime.Scoped:
                {
                    services.AddOptions<TOptions>()
                        .Bind(section)
                        .ValidateDataAnnotations()
                        .Validate(o => o.ValidateOptions(sectionName),
                                  $"{sectionName} configuration is invalid.");

                    services.AddScoped(sp =>
                        sp.GetRequiredService<IOptionsSnapshot<TOptions>>().Value);
                    break;
                }

            case OptionsLifetime.Monitor:
                {
                    services.AddOptions<TOptions>()
                        .Bind(section)
                        .ValidateDataAnnotations()
                        .Validate(o => o.ValidateOptions(sectionName),
                                  $"{sectionName} configuration is invalid.");

                    services.AddSingleton(sp =>
                        sp.GetRequiredService<IOptionsMonitor<TOptions>>().CurrentValue);
                    break;
                }
        }

        return services;
    }

    private static bool ValidateOptions<T>(this T? option, string optionName)
        where T : class
    {
        if (option is null)
            return false;

        var context = new ValidationContext(option);
        Validator.ValidateObject(option, context, validateAllProperties: true);

        return true;
    }
}

public enum OptionsLifetime
{
    Singleton,
    Scoped,
    Monitor
}

