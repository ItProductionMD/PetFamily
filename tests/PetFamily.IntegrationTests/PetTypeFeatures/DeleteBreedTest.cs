using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Commands.PetTypeManagement.DeleteBreed;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;

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

        var command = new DeleteBreedCommand(seedSpecies.Id, seededBreed.Id);
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
