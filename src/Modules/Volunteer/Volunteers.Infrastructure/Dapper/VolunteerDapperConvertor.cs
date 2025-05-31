using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedInfrastructure.Shared.Dapper;
using Volunteers.Application.ResponseDtos;
using static PetFamily.SharedInfrastructure.Shared.Dapper.Convertors;

namespace Volunteers.Infrastructure.Dapper;

public static class VolunteerDapperConvertor
{
    public static void Register()
    {
        SqlMapper.AddTypeHandler(new JsonbTypeHandler<List<SocialNetworksDto>>());
        SqlMapper.AddTypeHandler(new JsonbTypeHandler<List<RequisitesDto>>());
        SqlMapper.AddTypeHandler(new JsonbTypeHandler<List<PetMainInfoDto>>());
    }
}
