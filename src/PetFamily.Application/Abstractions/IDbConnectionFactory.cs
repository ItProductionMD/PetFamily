using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Application.Abstractions;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    Task<DbConnection> CreateOpenConnectionAsync();
}
