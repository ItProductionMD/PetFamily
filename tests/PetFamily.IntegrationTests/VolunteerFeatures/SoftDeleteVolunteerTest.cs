using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.Fixtures;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using Volunteers.Application.Commands.VolunteerManagement.SoftDeleteVolunteer;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class SoftDeleteVolunteerTest(
    TestWebApplicationFactory factory) : CommandHandlerTest<Guid, SoftDeleteVolunteerCommand>(factory)
{

    [Fact]
    public async Task Should_soft_delete_one_volunteer_successfully()
    {
        //ARRANGE
        var species = new SpeciesTestBuilder()
            .WithBreeds(["breedOne", "breedTwo"]).Species;
        await DbContextSeedExtensions.SeedAsync(_speciesDbContext, species);

        var seedVolunteer = new VolunteerTestBuilder()
            .WithPets(10, species).Volunteer;
        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, seedVolunteer);

        //ACT
        var result = await _sut.Handle(new(seedVolunteer.Id), CancellationToken.None);

        //ASSERT
        Assert.True(result.IsSuccess);

        var softDeletedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(x => x.Pets)
            .FirstOrDefaultAsync(x => x.Id == seedVolunteer.Id);

        Assert.NotNull(softDeletedVolunteer);
        Assert.True(softDeletedVolunteer.IsDeleted);

        foreach (var pet in softDeletedVolunteer.Pets)
        {
            Assert.True(pet.IsDeleted);
            Assert.NotNull(pet.DeletedAt);
        }
    }
}
