using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Commands.PetTypeManagment;
using PetFamily.Application.Commands.PetManagment.AddPet;
using PetFamily.Application.Commands.PetManagment.ChangePetsOrder;
using PetFamily.Application.Commands.PetManagment.UpdatePetImages;
using PetFamily.Application.Commands.PetManagment.UpdateSocialNetworks;
using PetFamily.Application.Queries;
using PetFamily.Application.SharedValidations;
using PetFamily.Application.Commands.VolunteerManagment.CreateVolunteer;
using PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;
using PetFamily.Application.Commands.VolunteerManagment.GetVolunteers;
using PetFamily.Application.Commands.VolunteerManagment.GetVolunteer;
using PetFamily.Application.Commands.VolunteerManagment.RestoreVolunteer;
using PetFamily.Application.Commands.VolunteerManagment.UpdateRequisites;
using PetFamily.Application.Commands.VolunteerManagment.UpdateVolunteer;
using PetFamily.Application.Commands.FilesManagment;

namespace PetFamily.Application;

public static class Inject
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(Inject).Assembly);
        services.AddScoped<CreateVolunteerHandler>();
        services.AddScoped<UpdateVolunteerHandler>();
        services.AddScoped<GetVolunteerHandler>();
        services.AddScoped<DeleteVolunteerHandler>();
        services.AddScoped<UpdateSocialNetworkHandler>();
        services.AddScoped<RestoreVolunteerHandler>();
        services.AddScoped<SoftDeleteVolunteerHandler>();
        services.AddScoped<AddPetHandler>();
        services.AddScoped<UpdateRequisitesHandler>();
        services.AddScoped<AddSpeciesHandler>();
        services.AddScoped<UpdatePetImagesHandler>();
        services.AddScoped<ChangePetsOrder>();
        services.AddScoped<GetVolunteersHandler>();
        services.AddSingleton<FilesProcessingQueue>();
        services.AddScoped<GetVolunteersQueryHandler>();
        services.Configure<FileValidatorOptions>(configuration.GetSection("FileValidatorOptions"));
        services.Configure<FileFolders>(configuration.GetSection("FileFolders"));
        
        return services;
    }
}
