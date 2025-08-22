using Microsoft.EntityFrameworkCore;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.Discussions.Infrastructure.Contexts;
using PetFamily.IntegrationTests.IClassFixtures;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;
using Volunteers.Domain;
using Volunteers.Domain.ValueObjects;
using Volunteers.Infrastructure.Contexts;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class UpdateVolunteerTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<Volunteer, UpdateVolunteerCommand>(factory)
{
    [Fact]
    public async Task Should_update_volunteer_correctly()
    {
        //ARRANGE
        var userId = Guid.NewGuid();
        var volunteerId = Guid.NewGuid();

        var volunteer = Volunteer.Create(
            VolunteerID.Create(volunteerId),
            UserId.Create(userId).Data!,
            FullName.Create("FirstName", "LastName").Data!,
            1,
            "description",
            Phone.CreateNotEmpty("2000111", "+383").Data!,
            []).Data!;

        await SeedAsync(typeof(VolunteerWriteDbContext), volunteer);

        var command = new UpdateVolunteerCommand(
            volunteer.UserId.Value,
            volunteer.Id,
            "updatedFirstName",
            "updatedLastName",
            "updated description",
            10);
        //ACT
        var updateResult = await _sut.Handle(command, CancellationToken.None);
        //ARRANGE
        Assert.True(updateResult.IsSuccess);

        var updatedVolunteer = await _volunteerDbContext.Volunteers
            .FirstOrDefaultAsync(v => v.Id == volunteer.Id);

        AssertCustom.AreEqualData(command, updatedVolunteer);
    }
}
