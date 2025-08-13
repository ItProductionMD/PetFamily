using Dapper;

namespace PetFamily.SharedInfrastructure.Dapper.Extensions;

public static class DynamicParametersExtensions
{
    public static Dictionary<string, object?> ToDictionary(this DynamicParameters parameters)
    {
        var result = new Dictionary<string, object?>();
        foreach (var paramName in parameters.ParameterNames)
        {
            result[paramName] = parameters.Get<object>(paramName);
        }
        return result;
    }
}