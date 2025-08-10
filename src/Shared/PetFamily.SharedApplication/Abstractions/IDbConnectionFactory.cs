using System.Data;
using System.Data.Common;

namespace PetFamily.SharedApplication.Abstractions;


public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    Task<DbConnection> CreateOpenConnectionAsync();
}
