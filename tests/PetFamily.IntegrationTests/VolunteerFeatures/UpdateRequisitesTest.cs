using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using PetFamily.SharedApplication.Dtos;
using Volunteers.Application.Commands.VolunteerManagement.UpdateRequisites;
using Volunteers.Application.ResponseDtos;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class UpdateRequisitesTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<UpdateRequisitesCommand>(factory)
{
    [Fact]
    public async Task Should_update_requisites_correctly()
    {
        //ARRANGE
        var seedVolunteer = new VolunteerTestBuilder().Volunteer;
        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, seedVolunteer);

        var command = new UpdateRequisitesCommand(
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
