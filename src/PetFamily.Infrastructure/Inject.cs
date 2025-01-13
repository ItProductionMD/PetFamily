using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Volunteers;
using PetFamily.Infrastructure.Repositories;

namespace PetFamily.Infrastructure;

public static class Inject
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    { 
        services
            .AddScoped<IVolunteerRepository, VolunteerRepository>()
            .AddScoped<AppDbContext>();
        return services;
    }
}
