using Dapper;
using Newtonsoft.Json;
using System.Data;

namespace PetFamily.SharedInfrastructure.Shared.Dapper;

public static class DapperMappers
{
    public class JsonbTypeMapper<T> : SqlMapper.TypeHandler<T>
    {
        public override void SetValue(IDbDataParameter parameter, T value)
        {
            parameter.Value = value == null
                ? (object)DBNull.Value
                : JsonConvert.SerializeObject(value);

            parameter.DbType = DbType.String;
        }

        public override T Parse(object value)
        {
            return value == null || value is DBNull
                ? default!
                : JsonConvert.DeserializeObject<T>(value.ToString()!)!;
        }
    }


}
