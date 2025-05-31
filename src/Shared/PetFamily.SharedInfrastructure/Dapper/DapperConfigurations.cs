using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetSpecies.Public.Dtos;

namespace PetFamily.SharedInfrastructure.Shared.Dapper;

public static class DapperConfigurations
{

    public static IServiceCollection ConfigDapper(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        SqlMapper.AddTypeHandler(new Convertors.JsonbTypeHandler<List<BreedDto>>());
        SqlMapper.AddTypeHandler(new Convertors.JsonbTypeHandler<List<SpeciesDto>>());

        services.Configure<DapperOptions>(configuration.GetSection("DapperOptions"));
        return services;
    }
}
