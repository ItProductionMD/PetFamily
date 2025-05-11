using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetFamily.Infrastructure.Contexts;
using static PetFamily.Tools.ToolsExtensions;
using PetFamily.Domain.PetTypeManagment.Root;

namespace PetFamily.Tools;

public static class Seeder
{
    /// <summary>
    /// Seed random data to db : Species and breeds, volunteers and pets
    /// </summary>
    /// <param name="volunteersCount"></param>
    /// <param name="volunteerPetsCount"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task RunSeed(int volunteersCount, int volunteerPetsCount)
    {
        var (scope, _dbContext) = CreateDbContext();
        using (scope)
        {
            var speciesList = new List<Species>();
            if (await _dbContext.AnimalTypes.AnyAsync() == false)
            {
                Console.WriteLine("###Preparing Species data...###");
                speciesList = SpeciesBuilder.Build();
                await _dbContext.AnimalTypes.AddRangeAsync(speciesList);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine("###seed species to db...###");
            }
            else
            {
                Console.WriteLine("###Species already exists!###");
                speciesList = await _dbContext.AnimalTypes
                    .Include(s=>s.Breeds)               
                    .ToListAsync();
            }
            Console.WriteLine("###Create Volunteers data");
            var volunteers = VolunteerBuilder.Build(volunteersCount, volunteerPetsCount, speciesList);

            Console.WriteLine($"###Created volunteers({volunteers.Count}) and pets({volunteers[0].Pets.Count})###");

            await _dbContext.Volunteers.AddRangeAsync(volunteers);
            await _dbContext.SaveChangesAsync();

            Console.WriteLine("###seed all data to db is ok!###");
        }
    }

    /// <summary>
    /// Clear indicated table in db
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static async Task RunClear(string tableName)
    {
        var (scope, dbContext) = CreateDbContext();
        using (scope)
        {
            switch (tableName.ToLower())
            {
                case "volunteers":
                    await dbContext.Volunteers.ExecuteDeleteAsync();
                    Console.WriteLine($"###Table {tableName} cleared successfully!###");
                    break;

                case "species":
                    await dbContext.AnimalTypes.ExecuteDeleteAsync();
                    Console.WriteLine($"###Table {tableName} cleared successfully!###");
                    break;

                default:
                    Console.WriteLine("###No souch table was found!###");
                    break;
            }
        }
    }

    private static (IServiceScope scope, WriteDbContext context) CreateDbContext()
    {
        var connectionString = GetConnectionString();
        Console.WriteLine("###Creating host...###");

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddScoped<WriteDbContext>(_ =>
                new WriteDbContext(connectionString));
            })
       .Build();

        var scope = host.Services.CreateScope();

        Console.WriteLine("###Getting dbContext...###");

        var _dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        if (_dbContext == null)
            throw new Exception("###_dbContext is null!###");

        return (scope, _dbContext);
    }
}
