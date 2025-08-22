using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.DbContextExtensions;
using PetFamily.IntegrationTests.IClassFixtures;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using PetSpecies.Application.Commands.DeleteSpecies;

namespace PetFamily.IntegrationTests.PetTypeFeatures;

public class DeleteSpeciesTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<DeleteSpeciesCommand>(factory)
{

    [Fact]
    public async Task Should_delete_species_correctly()
    {
        //ARRANGE
        var seedSpecies = new SpeciesTestBuilder().Species;
        await DbContextSeeder.SeedAsync(_speciesDbContext, seedSpecies);

        var command = new DeleteSpeciesCommand(seedSpecies.Id);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.True(result.IsSuccess);

        var deletedSpecies = await _speciesDbContext.AnimalTypes.FirstOrDefaultAsync();
        Assert.Null(deletedSpecies);
    }
}
