using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.Fixtures;
using PetFamily.IntegrationTests.WebApplicationFactory;
using PetSpecies.Application.Commands.AddSpecies;
using PetSpecies.Domain;

namespace PetFamily.IntegrationTests.PetTypeFeatures;

public class AddPetTypeTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<Species, AddPetTypeComand>(factory)
{
    [Fact]
    public async Task Should_add_pet_type_correctly()
    {
        //ARRANGE
        var speciesName = "testSpecies";
        var breedName = "testBreed";
        var breedDescription = "test breed description";
        var command = new AddPetTypeComand(speciesName, [new(breedName, breedDescription)]);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        var addedPetType = await _speciesDbContext.AnimalTypes
            .AsNoTracking()
            .Include(s => s.Breeds)
            .FirstOrDefaultAsync();

        Assert.NotNull(addedPetType);
        Assert.True(result.IsSuccess);
        Assert.Equal(speciesName, addedPetType.Name);
        Assert.Single(addedPetType.Breeds);
        Assert.Equal(breedName, addedPetType.Breeds[0].Name);
        Assert.Equal(breedDescription, addedPetType.Breeds[0].Description);

    }
}
