using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetTypeManagment.Root;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class HardDeleteVolunteerTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<Guid, HardDeleteVolunteerCommand>(factory)
{

    [Fact]
    public async Task Should_hard_delete_one_volunteer_successfully()
    {
        //ARRANGE
        var species = new SpeciesTestBuilder()
            .WithBreeds(["breedOne", "breedTwo"]).Species;
        await Seeder.Seed(species, _dbContext);

        var seedVolunteer = new VolunteerTestBuilder()
            .WithPets(10, species).Volunteer;
        await Seeder.Seed(seedVolunteer, _dbContext);

        var command = new HardDeleteVolunteerCommand(seedVolunteer.Id);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.True(result.IsSuccess);

        var hardDeletedVolunteer = await _dbContext.Volunteers
            .AsNoTracking()
            .Include(x => x.Pets)
            .FirstOrDefaultAsync(x => x.Id == seedVolunteer.Id);

        Assert.Null(hardDeletedVolunteer);
    }
}
