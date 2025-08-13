using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.IClassFixtures;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using Volunteers.Application.Queries.GetVolunteer;
using Volunteers.Application.ResponseDtos;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class GetVolunteerTest(TestWebApplicationFactory factory)
    : QueryHandlerTest<VolunteerDto, GetVolunteerQuery>(factory)
{
    [Fact]
    public async Task Should_get_volunteer_correctly()
    {
        //ARRANGE
        var seedVolunteer = new VolunteerTestBuilder(volunteersCount: 1).Volunteer;

        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, seedVolunteer);

        var query = new GetVolunteerQuery(seedVolunteer.Id);
        //ACT
        var result = await _sut.Handle(query, CancellationToken.None);
        //ASSERT
        Assert.True(result.IsSuccess);

        var addedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == seedVolunteer.Id);

        Assert.NotNull(addedVolunteer);
        Assert.Equal(seedVolunteer.Id, addedVolunteer.Id);
    }
}
