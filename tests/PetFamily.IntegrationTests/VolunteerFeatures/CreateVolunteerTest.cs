using Microsoft.EntityFrameworkCore;
using Moq;
using PetFamily.IntegrationTests.Fixtures;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using PetFamily.SharedApplication.IUserContext;
using Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class CreateVolunteerTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<Guid, CreateVolunteerCommand>(factory)
{
    [Fact]
    public async Task Should_create_one_volunteer_successfully()
    {
        //ARRANGE
        var command = new CreateVolunteerCommand(
            Guid.NewGuid(),
            "Iurii",
            "Godina",
            "description",
            1,
            "+39",
            "00000000",
            [new("victoriabank", "iban12345")]);

        //ACT
        var handleResult = await _sut.Handle(command, CancellationToken.None);

        //ASSERT
        Assert.True(handleResult.IsSuccess);

        var addedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .FirstOrDefaultAsync();

        AssertCustom.AreEqualData(command, addedVolunteer);
    }
}
