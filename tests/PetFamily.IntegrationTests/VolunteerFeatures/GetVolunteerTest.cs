using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Dtos;
using PetFamily.Application.Queries.Volunteer.GetVolunteer;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class GetVolunteerTest(TestWebApplicationFactory factory) 
    : QueryHandlerTest<VolunteerDto, GetVolunteerQuery>(factory)
{
    [Fact]
    public async Task Should_get_volunteer_correctly()
    {
        //ARRANGE
        var seedVolunteer = new VolunteerTestBuilder().Volunteer;

        await Seeder.Seed(seedVolunteer, _dbContext);

        var query = new GetVolunteerQuery(seedVolunteer.Id);
        //ACT
        var result = await _sut.Handle(query, CancellationToken.None);
        //ASSERT
        Assert.True(result.IsSuccess);

        var addedVolunteer = await _dbContext.Volunteers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == seedVolunteer.Id);

        Assert.NotNull(addedVolunteer);
        Assert.Equal(seedVolunteer.Id, addedVolunteer.Id);
    }
}
