using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;

public static class PermissionsTable
{
    public const string TableName = "auth.permissions";
    public const string Id = "id";
    public const string Code = "code";
    public const string IsEnabled = "is_enabled";
    public const string ConstraintName = "permission_id";
}
