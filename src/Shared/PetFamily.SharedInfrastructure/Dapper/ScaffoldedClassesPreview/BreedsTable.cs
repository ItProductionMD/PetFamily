using PetFamily.SharedInfrastructure.Constants;

namespace PetFamily.SharedInfrastructure.Shared.Dapper.ScaffoldedClassesPreview;

public static class BreedsTable
{
    public const string TableName = "breeds";
    public const string TableFullName = "species.breeds";
    public const string Id = "id";
    public const string Name = "name";
    public const string Description = "description";
    public const string SpeciesId = "species_id";
}
