using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.PaginationUtils.PagedResult;
using static PetFamily.SharedInfrastructure.Dapper.Extensions.DapperLoggerExtensions;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using PetFamily.SharedInfrastructure.Dapper.Extensions;

namespace PetFamily.SharedInfrastructure.Dapper.Extensions;

public static class DapperPagingExtensions
{
    public static async Task<(int, IReadOnlyList<T>)> GetPagedQuery<T,Repo>(
        this IDbConnection connection,
        CommandDefinition baseCommand,
        ILogger<Repo> logger)
    {
        var countSql = $@"
            SELECT COUNT(*) AS TotalCount
            FROM ({baseCommand.CommandText}) AS BaseQuery;";

        var countSqlCmd = new CommandDefinition(
            countSql,
            baseCommand.Parameters,
            cancellationToken: baseCommand.CancellationToken
        );

        var itemsSql = $@"
            SELECT *
            FROM ({baseCommand.CommandText}) AS BaseQuery
            LIMIT @PageSize OFFSET @Offset;";

        var itemsSqlCmd = new CommandDefinition(
            itemsSql,
            baseCommand.Parameters,
            cancellationToken: baseCommand.CancellationToken
        );

        logger.DapperLogInformation(countSqlCmd.CommandText, baseCommand.Parameters);
        var totalCount = await connection.ExecuteScalarAsync<int>(countSqlCmd);

        logger.DapperLogInformation(itemsSqlCmd.CommandText, baseCommand.Parameters);
        var items = (await connection.QueryAsync<T>(itemsSqlCmd)).ToList();

        return (totalCount, items);
    }

    

    private static T MapDynamic<T>(dynamic row)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(row);
        var myObject = System.Text.Json.JsonSerializer.Deserialize<T>(json)!;

        return myObject;
    }

    private static Dictionary<string, object?> ToDictionary(this object obj)
    {
        return obj.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name, p => p.GetValue(obj));
    }
    
}
