using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.Fixtures;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;
using Volunteers.Domain;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class UpdateVolunteerTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<Volunteer, UpdateVolunteerCommand>(factory)
{
    [Fact]
    public async Task Should_update_volunteer_correctly()
    {
        //ARRANGE
        var seedVolunteer = new VolunteerTestBuilder().Volunteer;
        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, seedVolunteer);

        var command = new UpdateVolunteerCommand(
            seedVolunteer.Id,
            "updatedFirstName",
            "updatedLastName",
            "updated description",
            10);
        //ACT
        var updateResult = await _sut.Handle(command, CancellationToken.None);
        //ARRANGE
        Assert.True(updateResult.IsSuccess);

        var updatedVolunteer = await _volunteerDbContext.Volunteers
            .FirstOrDefaultAsync(v => v.Id == seedVolunteer.Id);

        AssertCustom.AreEqualData(command, updatedVolunteer);
    }
}
