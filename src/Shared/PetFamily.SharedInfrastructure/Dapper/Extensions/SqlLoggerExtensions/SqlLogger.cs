using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace PetFamily.SharedInfrastructure.Dapper.Extensions.SqlLoggerExtensions;
public class SqlLogger
{
    private readonly ILogger<SqlLogger> _logger;
    private readonly IHostEnvironment _env;

    private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "token", "accessToken", "secret", "userId", "email"
    };

    public SqlLogger(ILogger<SqlLogger> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public void Log(string sql, object? parameters = null)
    {
        if (!_logger.IsEnabled(LogLevel.Information))
            return;

        string maskedParams = parameters == null
            ? "none"
            : FormatParams(parameters);

        _logger.LogInformation("SQL Executed:\n{Sql}\nParameters: {Params}", sql, maskedParams);
    }

    private string FormatParams(object parameters)
    {
        var props = parameters.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        return string.Join(", ", props.Select(p =>
        {
            var key = p.Name;
            var value = p.GetValue(parameters);

            if (_env.IsProduction() && SensitiveKeys.Contains(key))
                return $"{key}=***";

            return $"{key}={value}";
        }));
    }
}

