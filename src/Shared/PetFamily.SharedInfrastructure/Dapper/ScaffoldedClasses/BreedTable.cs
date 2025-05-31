using PetFamily.SharedInfrastructure.Constants;

namespace PetFamily.SharedInfrastructure.Shared.Dapper.ScaffoldedClasses;
public static class BreedTable
{
    public const string TableName = "breeds";
    public const string TableFullName = SchemaNames.SPECIES + "." + TableName;
    public const string Id = "id";
    public const string Name = "name";
    public const string Description = "description";
    public const string SpeciesId = "species_id";
}
