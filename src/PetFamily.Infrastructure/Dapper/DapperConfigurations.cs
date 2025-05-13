using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Dtos;
using static PetFamily.Infrastructure.Dapper.Convertors;

namespace PetFamily.Infrastructure.Dapper;

public static class DapperConfigurations
{

    public static IServiceCollection ConfigDapper(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        SqlMapper.AddTypeHandler(new JsonbTypeHandler<List<SocialNetworksDto>>());
        SqlMapper.AddTypeHandler(new JsonbTypeHandler<List<RequisitesDto>>());
        SqlMapper.AddTypeHandler(new JsonbTypeHandler<List<PetMainInfoDto>>());
        SqlMapper.AddTypeHandler(new JsonbTypeHandler<List<BreedDto>>());

        services.Configure<DapperOptions>(configuration.GetSection("DapperOptions"));
        return services;
    }
}
