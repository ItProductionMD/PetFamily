using PetFamily.SharedInfrastructure.Constants;

namespace PetFamily.SharedInfrastructure.Shared.Dapper.ScaffoldedClasses;

public static class SpeciesTable
{
    public const string TableName = "species";
    public const string TableFullName = SchemaNames.SPECIES + "." + TableName;
    public const string Id = "id";
    public const string Name = "name";
}
