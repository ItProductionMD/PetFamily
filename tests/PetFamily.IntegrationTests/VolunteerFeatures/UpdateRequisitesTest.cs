using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.DbContextExtensions;
using PetFamily.IntegrationTests.IClassFixtures;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using PetFamily.SharedApplication.Dtos;
using Volunteers.Application.Commands.VolunteerManagement.UpdateRequisites;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class UpdateRequisitesTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<UpdateRequisitesCommand>(factory)
{
    [Fact]
    public async Task Should_update_requisites_correctly()
    {
        //ARRANGE
        var seedVolunteer = new VolunteerTestBuilder().Volunteer;
        await DbContextSeeder.SeedAsync(_volunteerDbContext, seedVolunteer);

        var command = new UpdateRequisitesCommand(
            seedVolunteer.UserId.Value,
            seedVolunteer.Id,
            [
                new RequisitesDto("testName","test description"),
                new RequisitesDto("testNameTwo", "test description two")
            ]);

        //ACT
        var updateRequisitesResult = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(updateRequisitesResult);
        Assert.True(updateRequisitesResult.IsSuccess);

        var updatedVolunteer = await _volunteerDbContext.Volunteers
            .FirstOrDefaultAsync(v => v.Id == seedVolunteer.Id);

        Assert.NotNull(updatedVolunteer);
        Assert.Equal(2, updatedVolunteer.Requisites.Count);
        Assert.Equal("testName", updatedVolunteer.Requisites[0].Name);
        Assert.Equal("test description", updatedVolunteer.Requisites[0].Description);
    }
}
