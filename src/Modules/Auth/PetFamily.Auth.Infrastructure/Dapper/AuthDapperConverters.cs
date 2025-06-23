using Dapper;
using PetFamily.Auth.Application.Dtos;
using static PetFamily.SharedInfrastructure.Shared.Dapper.Convertors;

namespace PetFamily.Auth.Infrastructure.Dapper;

public static class AuthDapperConverters
{
    public static void Register()
    {
        SqlMapper.AddTypeHandler(new JsonbTypeHandler<List<PermissionDto>>());
    }
}
