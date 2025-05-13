using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Commands.VolunteerManagment.UpdateVolunteer;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class UpdateVolunteerTest(TestWebApplicationFactory factory) 
    : CommandHandlerTest<Volunteer, UpdateVolunteerCommand>(factory)
{
    [Fact]
    public async Task Should_update_volunteer_correctly()
    {
        //ARRANGE
        var seedVolunteer = new VolunteerTestBuilder().Volunteer;
        await Seeder.Seed(seedVolunteer, _dbContext);

        var command = new UpdateVolunteerCommand(
            seedVolunteer.Id,
            "updatedFirstName",
            "updatedLastName",
            "updatedEmail@gmail.com",
            "updated description",
            "06666666",
            "+333",
            10);
        //ACT
        var updateResult = await _sut.Handle(command, CancellationToken.None);
        //ARRANGE
        Assert.True(updateResult.IsSuccess);

        var updatedVolunteer = await _dbContext.Volunteers
            .FirstOrDefaultAsync(v => v.Id == seedVolunteer.Id);

        AssertCustom.AreEqualData(command, updatedVolunteer);
    }
}
