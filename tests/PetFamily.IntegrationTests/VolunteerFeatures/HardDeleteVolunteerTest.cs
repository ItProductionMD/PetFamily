using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using Volunteers.Application.Commands.VolunteerManagement.DeleteVolunteer;

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
        await DbContextSeedExtensions.SeedAsync(_speciesDbContext, species);

        var volunteer = new VolunteerTestBuilder()
            .WithPets(10, species).Volunteer;
        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, volunteer);

        var command = new HardDeleteVolunteerCommand(volunteer.Id);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.True(result.IsSuccess);

        var hardDeletedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(x => x.Pets)
            .FirstOrDefaultAsync(x => x.Id == volunteer.Id);

        Assert.Null(hardDeletedVolunteer);
    }
}
