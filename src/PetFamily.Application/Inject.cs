using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Commands.PetTypeManagment;
using PetFamily.Application.Commands.PetManagment.AddPet;
using PetFamily.Application.Commands.PetManagment.ChangePetsOrder;
using PetFamily.Application.Commands.PetManagment.UpdatePetImages;
using PetFamily.Application.Commands.PetManagment.UpdateSocialNetworks;
using PetFamily.Application.SharedValidations;
using PetFamily.Application.Commands.VolunteerManagment.CreateVolunteer;
using PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;
using PetFamily.Application.Commands.VolunteerManagment.GetVolunteers;
using PetFamily.Application.Commands.VolunteerManagment.RestoreVolunteer;
using PetFamily.Application.Commands.VolunteerManagment.UpdateRequisites;
using PetFamily.Application.Commands.VolunteerManagment.UpdateVolunteer;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Queries.Volunteer.GetVolunteers;
using PetFamily.Application.Queries.Volunteer.GetVolunteer;
using System.Runtime.InteropServices;
using PetFamily.Application.Abstractions;

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
