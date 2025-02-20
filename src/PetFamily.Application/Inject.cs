﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Application.Volunteers.DeleteVolunteer;
using PetFamily.Application.Volunteers.GetVolunteer;
using PetFamily.Application.Volunteers.RestoreVolunteer;
using PetFamily.Application.Volunteers.UpdateSocialNetworks;
using PetFamily.Application.Volunteers.UpdateVolunteer;

namespace PetFamily.Application;

public static class Inject
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(Inject).Assembly);
        services.AddScoped<CreateVolunteerHandler>();
        services.AddScoped<UpdateVolunteerHandler>();
        services.AddScoped<GetVolunteerHandler>();
        services.AddScoped<DeleteVolunteerHandler>();
        services.AddScoped<UpdateSocialNetworkHandler>();
        services.AddScoped<RestoreVolunteerHandler>();
        services.AddScoped<SoftDeleteVolunteerHandler>();

        return services;
    }
}
