using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedApplication.Dtos;
using PetFamily.SharedInfrastructure.Shared.Dapper;
using Volunteers.Application.ResponseDtos;
using static PetFamily.SharedInfrastructure.Shared.Dapper.DapperMappers;

namespace Volunteers.Infrastructure.Dapper;

public static class VolunteerDapperConvertor
{
    public static void Register()
    {
        SqlMapper.AddTypeHandler(new JsonbTypeMapper<List<SocialNetworksDto>>());
        SqlMapper.AddTypeHandler(new JsonbTypeMapper<List<RequisitesDto>>());
        SqlMapper.AddTypeHandler(new JsonbTypeMapper<List<PetMainInfoDto>>());
    }
}
