using Microsoft.EntityFrameworkCore;

namespace PetFamily.IntegrationTests.Seeds;

public static class DbContextSeedExtensions
{
    public static async Task SeedAsync<T, TContext>(this TContext dbContext, T entity)
        where T : class
        where TContext : DbContext
    {
        dbContext.Set<T>().Add(entity);
        await dbContext.SaveChangesAsync();
    }

    public static async Task SeedRangeAsync<T, TContext>(this TContext dbContext, List<T> entity)
        where T : class
        where TContext : DbContext
    {
        dbContext.Set<T>().AddRange(entity);
        await dbContext.SaveChangesAsync();
    }
}
