using System.Data;
using System.Data.Common;

namespace PetFamily.Application.Abstractions;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    Task<DbConnection> CreateOpenConnectionAsync();
}
