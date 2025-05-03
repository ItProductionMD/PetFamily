using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using PetFamily.Application.Commands.PetTypeManagement.AddPetType;
using PetFamily.Application.Commands.PetTypeManagement.DeleteBreed;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetTypeManagment.Entities;
using PetFamily.Domain.PetTypeManagment.Root;
using PetFamily.Infrastructure.Contexts;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using System.Linq;

namespace PetFamily.IntegrationTests.PetTypeFeatures;

public class DeleteBreedTest(TestWebApplicationFactory factory) 
    : CommandHandlerTest<DeleteBreedCommand>(factory)
{
    [Fact]
    public async Task Should_delete_breed_correctly()
    {
        //ARRANGE
        var breedName = "testBreed";
        var seedSpecies = new SpeciesTestBuilder()
            .WithBreeds([breedName]).Species;
        await Seeder.Seed(seedSpecies, _dbContext);

        var seededBreed = seedSpecies.Breeds[0];

        var command = new DeleteBreedCommand(seedSpecies.Id,seededBreed.Id);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.True(result.IsSuccess);

        var updatedSpecies = await _dbContext.AnimalTypes
           .AsNoTracking()
           .Include(b => b.Breeds)
           .SingleOrDefaultAsync();

        Assert.NotNull(updatedSpecies);
        Assert.Empty(seedSpecies.Breeds);
    }
}
