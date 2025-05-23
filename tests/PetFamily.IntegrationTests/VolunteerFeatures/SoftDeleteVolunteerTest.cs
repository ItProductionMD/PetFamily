﻿using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;

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
        await Seeder.Seed(species, _dbContext);

        var seedVolunteer = new VolunteerTestBuilder()
            .WithPets(10, species).Volunteer;
        await Seeder.Seed(seedVolunteer, _dbContext);

        //ACT
        var result = await _sut.Handle(new(seedVolunteer.Id), CancellationToken.None);

        //ASSERT
        Assert.True(result.IsSuccess);

        var softDeletedVolunteer = await _dbContext.Volunteers
            .AsNoTracking()
            .Include(x => x.Pets)
            .FirstOrDefaultAsync(x => x.Id == seedVolunteer.Id);

        Assert.NotNull(softDeletedVolunteer);
        Assert.True(softDeletedVolunteer.IsDeleted);

        foreach (var pet in softDeletedVolunteer.Pets)
        {
            Assert.True(pet.IsDeleted);
            Assert.NotNull(pet.DeletedDateTime);
        }
    }
}
