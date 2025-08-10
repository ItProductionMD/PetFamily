using Dapper;
using PetFamily.Auth.Application.Dtos;
using static PetFamily.SharedInfrastructure.Shared.Dapper.DapperMappers;

namespace PetFamily.Auth.Infrastructure.Dapper;

public static class AuthDapperConverters
{
    public static void Register()
    {
        SqlMapper.AddTypeHandler(new JsonbTypeMapper<List<PermissionDto>>());
    }
}
