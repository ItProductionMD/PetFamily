

using Dapper;
using PetFamily.Application.Dtos;
using static PetFamily.Infrastructure.Dapper.Convertors;

namespace PetFamily.Infrastructure.Dapper;

public static class DapperConfigurations
{
    public static void ConfigDapper()
    {
        SqlMapper.AddTypeHandler(new JsonbTypeHandler<List<SocialNetworksDto>>());
        SqlMapper.AddTypeHandler(new JsonbTypeHandler<List<RequisitesDto>>());
        SqlMapper.AddTypeHandler(new JsonbTypeHandler<List<PetMainInfoDto>>());

    }
}
