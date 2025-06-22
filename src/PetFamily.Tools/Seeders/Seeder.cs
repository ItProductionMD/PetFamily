using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetSpecies.Domain;
using PetSpecies.Infrastructure.Contexts;
using Volunteers.Infrastructure.Contexts;
using static PetFamily.Tools.ToolsExtensions;

namespace PetFamily.Tools.Seeders;

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
        Console.WriteLine("###Preparing Species data...###");

        var speciesList = new List<Species>();
        speciesList = SpeciesBuilder.Build();

        var (speciesScope, speciesDbContext) = ContextsCreator.CreateSpeciesDbContext();

        using (speciesScope)
        {
            if (await speciesDbContext.AnimalTypes.AnyAsync() == false)
            {
                await speciesDbContext.AnimalTypes.AddRangeAsync(speciesList);
                await speciesDbContext.SaveChangesAsync();
                Console.WriteLine("###seed species to db...###");
            }
            else
            {
                Console.WriteLine("###Species already exists!###");
                speciesList = await speciesDbContext.AnimalTypes
                    .Include(s => s.Breeds)
                    .ToListAsync();
            }
        }

        Console.WriteLine("###Create Volunteers data");

        var volunteers = VolunteerBuilder.Build(volunteersCount, volunteerPetsCount, speciesList);

        Console.WriteLine($"###Created volunteers({volunteers.Count}) and pets({volunteers[0].Pets.Count})###");

        var (volunteerScope, _volunteerDbContext) = ContextsCreator.CreateVolunteerDbContext();

        using (volunteerScope)
        {
            await _volunteerDbContext.Volunteers.AddRangeAsync(volunteers);
            await _volunteerDbContext.SaveChangesAsync();

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
        var (speciesScope, speciesDbContext) = ContextsCreator.CreateSpeciesDbContext();
        var (volunteerScope, volunteerDbContext) = ContextsCreator.CreateVolunteerDbContext();

        using (volunteerScope)
        using (speciesScope)
        {
            switch (tableName.ToLower())
            {
                case "volunteers":
                    await volunteerDbContext.Volunteers.ExecuteDeleteAsync();
                    Console.WriteLine($"###Table {tableName} cleared successfully!###");
                    break;

                case "species":
                    await speciesDbContext.AnimalTypes.ExecuteDeleteAsync();
                    Console.WriteLine($"###Table {tableName} cleared successfully!###");
                    break;

                case "all":
                    await volunteerDbContext.Volunteers.ExecuteDeleteAsync();
                    await speciesDbContext.AnimalTypes.ExecuteDeleteAsync();
                    Console.WriteLine($"###All Tables cleared successfully!###");

                    break;

                default:
                    Console.WriteLine("###No souch table was found!###");
                    break;
            }
        }
    }
}

