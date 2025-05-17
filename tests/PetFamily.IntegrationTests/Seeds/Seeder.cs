using PetFamily.Infrastructure.Contexts;

namespace PetFamily.IntegrationTests.Seeds;

public static class Seeder
{
    public static async Task Seed<T>(this T testObject, WriteDbContext dbContext) where T : class
    {
        dbContext.Set<T>().Add(testObject);
        await dbContext.SaveChangesAsync();
    }
    public static async Task SeedRange<T>(this List<T> testObjects, WriteDbContext dbContext) where T : class
    {
        dbContext.Set<T>().AddRange(testObjects);
        await dbContext.SaveChangesAsync();
    }
}
