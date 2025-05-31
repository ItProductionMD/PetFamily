using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using Volunteers.Application.Commands.VolunteerManagement.RestoreVolunteer;
using Volunteers.Application.Commands.VolunteerManagement.SoftDeleteVolunteer;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class RestoreVolunteerTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<RestoreVolunteerCommand>(factory)
{
    [Fact]
    public async Task Should_restore_one_volunteer_successfully()
    {
        //ARRANGE
        var softDeleteHandler = GetCommandHandler<Guid, SoftDeleteVolunteerCommand>();

        var seedSpecies = new SpeciesTestBuilder()
            .WithBreeds(["breedOne", "breedTwo"]).Species;
        await DbContextSeedExtensions.SeedAsync(_speciesDbContext, seedSpecies);

        var seedVolunteer = new VolunteerTestBuilder()
            .WithPets(10, seedSpecies).Volunteer;
        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, seedVolunteer);

        await softDeleteHandler.Handle(new(seedVolunteer.Id), CancellationToken.None);

        var command = new RestoreVolunteerCommand(seedVolunteer.Id);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.True(result.IsSuccess);

        var restoredVolunteer = await _volunteerDbContext.Volunteers
            .Include(x => x.Pets)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == seedVolunteer.Id);

        Assert.NotNull(restoredVolunteer);
        Assert.False(restoredVolunteer.IsDeleted);
        Assert.Null(restoredVolunteer.DeletedDateTime);

        foreach (var pet in restoredVolunteer.Pets)
        {
            Assert.False(pet.IsDeleted);
            Assert.Null(pet.DeletedDateTime);
        }
    }
}
