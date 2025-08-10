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
        SqlMapper.AddTypeHandler(new DapperMappers.JsonbTypeMapper<List<BreedDto>>());
        SqlMapper.AddTypeHandler(new DapperMappers.JsonbTypeMapper<List<SpeciesDto>>());
        SqlMapper.AddTypeHandler(new DapperMappers.JsonbTypeMapper<List<string>>());
        SqlMapper.AddTypeHandler(new DapperMappers.JsonbTypeMapper<List<Guid>>());


        services.Configure<DapperOptions>(configuration.GetSection("DapperOptions"));
        return services;
    }
}
