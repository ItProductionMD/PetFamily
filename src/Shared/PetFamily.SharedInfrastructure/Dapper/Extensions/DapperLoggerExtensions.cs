using Microsoft.Extensions.Logging;

namespace PetFamily.SharedInfrastructure.Dapper.Extensions;

public static class DapperLoggerExtensions
{
    public static void DapperLogSqlQuery<Repo>(this ILogger<Repo> logger, string sql, object? parameters)
    {
        //if info mode, log the SQL command
        logger.LogInformation("EXECUTING QUERY: {CommandText}",
            sql);

        //if debug mode, log the SQL command and parameters
        logger.LogDebug("EXECUTING QUERY: {CommandText} with parameters: {@Parameters}",
            sql, parameters ?? new { });
    }
}
