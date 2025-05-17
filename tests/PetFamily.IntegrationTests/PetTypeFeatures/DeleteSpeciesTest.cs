using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Commands.PetTypeManagement.DeleteSpecies;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;

namespace PetFamily.IntegrationTests.PetTypeFeatures;

public class DeleteSpeciesTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<DeleteSpeciesCommand>(factory)
{

    [Fact]
    public async Task Should_delete_species_correctly()
    {
        //ARRANGE
        var seedSpecies = new SpeciesTestBuilder().Species;
        await Seeder.Seed(seedSpecies, _dbContext);

        var command = new DeleteSpeciesCommand(seedSpecies.Id);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.True(result.IsSuccess);

        var deletedSpecies = await _dbContext.AnimalTypes.FirstOrDefaultAsync();
        Assert.Null(deletedSpecies);
    }
}
