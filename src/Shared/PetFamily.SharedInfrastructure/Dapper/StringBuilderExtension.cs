using Dapper;
using System.Text;

namespace PetFamily.SharedInfrastructure.Shared.Dapper;

public static class StringBuilderExtension
{
    internal static StringBuilder AppendPagination(
       this StringBuilder sqlBuilder,
       int pageNumber,
       int pageSize,
       DynamicParameters parameters)
    {
        sqlBuilder.AppendLine(" LIMIT @PageSize OFFSET @Offset");
        parameters.Add("Offset", (pageNumber - 1) * pageSize);
        parameters.Add("PageSize", pageSize);
        return sqlBuilder;
    }
}
