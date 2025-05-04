
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.SharedCommands;
using PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetTypeManagment.Entities;
using PetFamily.Domain.PetTypeManagment.Root;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Infrastructure.Contexts;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;

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
        await Seeder.Seed(seedSpecies, _dbContext);

        var seedVolunteer = new VolunteerTestBuilder()
            .WithPets(10,seedSpecies).Volunteer;
        await Seeder.Seed(seedVolunteer, _dbContext);

        await softDeleteHandler.Handle(new(seedVolunteer.Id), CancellationToken.None);

        var command = new RestoreVolunteerCommand(seedVolunteer.Id);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.True(result.IsSuccess);

        var restoredVolunteer = await _dbContext.Volunteers
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
