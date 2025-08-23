using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedApplication.DependencyInjection;
using PetFamily.SharedInfrastructure.Shared.Constants;
using PetFamily.SharedInfrastructure.Shared.Dapper;
using PetFamily.VolunteerRequests.Application.Dtos;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Infrastructure.BGServices;
using PetFamily.VolunteerRequests.Infrastructure.Contexts;
using PetFamily.VolunteerRequests.Infrastructure.Repositories.Read;
using PetFamily.VolunteerRequests.Infrastructure.Repositories.Write;
using static PetFamily.VolunteerRequests.Application.VolunteerRequestApplicationInjector;

namespace PetFamily.VolunteerRequests.Infrastructure;

public static class VolunteerRequestInjector
{
    public static IServiceCollection AddVolunteerRequestModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.TryGetConnectionString(ConnectionStringName.POSTGRESQL);

        SqlMapper.AddTypeHandler(new DapperMappers.JsonbTypeMapper<List<VolunteerRequestDto>>());

        services
            .AddScoped<IVolunteerRequestWriteRepository, VolunteerRequestWriteRepository>()
            .AddScoped<IVolunteerRequestReadRepository, VolunteerRequestReadRepository>()
            .InjectVolunteerRequestApplication(configuration)
            .AddScoped<VolunteerRequestWriteDbContext>(_ => new VolunteerRequestWriteDbContext(postgresConnection))
            .AddHostedService<RejectedStatusProcessor>();

        return services;
    }
}