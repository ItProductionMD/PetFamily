using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using Volunteers.Application.Queries.GetVolunteers;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class GetVolunteersTest(TestWebApplicationFactory factory)
    : QueryHandlerTest<GetVolunteersResponse, GetVolunteersQuery>(factory)
{
    [Theory]
    [InlineData(25, 10, 1)]//the first page
    [InlineData(25, 25, 1)]//when the first page is the last
    [InlineData(20, 25, 1)]//the first uncompleted page
    [InlineData(25, 10, 3)]//the last uncompleted page 
    [InlineData(25, 10, 2)]//the middle page
    [InlineData(25, 10, 4)]//the invalid value of page(more than exists)
    [InlineData(25, 10, 0)]//the invalid value of page(0)
    [InlineData(25, 10, -1)]//the invalid value of page(negative)
    public async Task Should_get_volunteers_correctly(
        int volunteersCount,
        int pageSize,
        int pageNumber)
    {
        //ARRANGE
        int totalPages = (int)Math.Ceiling((double)volunteersCount / pageSize);

        int volunteersOnLastPage = pageSize - (totalPages * pageSize - volunteersCount);

        var query = new GetVolunteersQuery(pageSize, pageNumber, null, null);

        var volunteers = new VolunteerTestBuilder(volunteersCount: volunteersCount).Volunteers;

        await DbContextSeedExtensions.SeedRangeAsync(_volunteerDbContext, volunteers);
        //ACT
        var result = await _sut.Handle(query, CancellationToken.None);
        //ASSERT
        Assert.True(result.IsSuccess);
        Assert.Equal(volunteersCount, result.Data!.VolunteersCount);
        Assert.True(pageSize >= result.Data!.Volunteers.Count);
        //if page is not valid
        if (pageNumber > totalPages || pageNumber <= 0)
            Assert.Empty(result.Data!.Volunteers);
        //if it is the lastPage
        else if (pageNumber == totalPages)
            Assert.Equal(volunteersOnLastPage, result.Data!.Volunteers.Count);
        //if it is not the lastPage
        else if (pageNumber < totalPages)
            Assert.Equal(pageSize, result.Data!.Volunteers.Count);
    }
}
