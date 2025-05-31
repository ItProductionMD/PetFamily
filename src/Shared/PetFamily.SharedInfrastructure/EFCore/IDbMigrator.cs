namespace PetFamily.SharedInfrastructure.Shared.EFCore;

public interface IDbMigrator
{
    Task MigrateAsync();
}
