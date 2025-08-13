using Dapper;
using static PetFamily.SharedInfrastructure.Shared.Dapper.DapperMappers;

namespace PetFamily.Discussions.Infrastructure.Dapper;

public static class DiscussionMapperConvertors
{
    public static void Register()
    {
        SqlMapper.AddTypeHandler(new JsonbTypeMapper<IReadOnlyList<Guid>>());
    }

}
