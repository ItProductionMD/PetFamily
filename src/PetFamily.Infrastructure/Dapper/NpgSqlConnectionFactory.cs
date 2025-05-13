
using Microsoft.Extensions.Configuration;
using Npgsql;
using PetFamily.Application.Abstractions;
using System.Data;

namespace PetFamily.Infrastructure.Dapper;

public class NpgSqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgSqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
