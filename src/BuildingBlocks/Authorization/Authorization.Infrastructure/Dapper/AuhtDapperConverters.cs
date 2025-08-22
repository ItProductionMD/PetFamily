using Authorization.Application.Dtos;
using Dapper;
using static PetFamily.SharedInfrastructure.Shared.Dapper.DapperMappers;

namespace Authorization.Infrastructure.Dapper;

public static class AuthDapperConverters
{
    public static void Register()
    {
        SqlMapper.AddTypeHandler(new JsonbTypeMapper<List<PermissionDto>>());
    }
}
