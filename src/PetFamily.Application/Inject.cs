using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.SharedValidations;

namespace PetFamily.Application;

public static class Inject
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration) =>

        services.AddValidatorsFromAssembly(typeof(Inject).Assembly)
                .AddCommandsAndQueries()
                .AddSingleton<FilesProcessingQueue>()
                .Configure<FileValidatorOptions>(configuration.GetSection("FileValidatorOptions"))
                .Configure<FileFolders>(configuration.GetSection("FileFolders"));

    private static IServiceCollection AddCommandsAndQueries(this IServiceCollection services)
    {
        return services.Scan(scan => scan.FromAssemblies(typeof(Inject).Assembly)
                    .AddClasses(classes =>
                        classes.AssignableToAny(
                            typeof(ICommandHandler<,>),
                            typeof(ICommandHandler<>),
                            typeof(IQueryHandler<,>),
                            typeof(IQueryHandler<>)))
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime());
    }
}
