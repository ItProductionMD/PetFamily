using Dapper;
using Account.Application.Dtos;
using static PetFamily.SharedInfrastructure.Shared.Dapper.DapperMappers;

namespace Account.Infrastructure.Dapper;

public static class AuthDapperConverters
{
    public static void Register()
    {
        SqlMapper.AddTypeHandler(new JsonbTypeMapper<List<PermissionDto>>());
    }
}
