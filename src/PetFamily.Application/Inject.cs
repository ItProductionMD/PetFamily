using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Pets.CreatePet;
using PetFamily.Application.Pets.UpdatePetImages;
using PetFamily.Application.SharedValidations;
using PetFamily.Application.Species;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Application.Volunteers.DeleteVolunteer;
using PetFamily.Application.Volunteers.GetVolunteer;
using PetFamily.Application.Volunteers.RestoreVolunteer;
using PetFamily.Application.Volunteers.UpdateRequisites;
using PetFamily.Application.Volunteers.UpdateSocialNetworks;
using PetFamily.Application.Volunteers.UpdateVolunteer;

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
        services.Configure<FileValidatorOptions>(configuration.GetSection("FileValidatorOptions"));

        return services;
    }
}
