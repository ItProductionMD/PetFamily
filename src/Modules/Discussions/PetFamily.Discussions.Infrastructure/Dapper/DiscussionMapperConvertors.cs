using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PetFamily.SharedInfrastructure.Shared.Dapper.DapperMappers;

namespace PetFamily.Discussions.Infrastructure.Dapper;

public static class DiscussionMapperConvertors
{
    public static void Register()
    {
        SqlMapper.AddTypeHandler(new JsonbTypeMapper<IReadOnlyList<Guid>>());
    }

}
