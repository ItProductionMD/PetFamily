using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Infrastructure.Extensions;

internal static class DynamicParametersExtensions
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