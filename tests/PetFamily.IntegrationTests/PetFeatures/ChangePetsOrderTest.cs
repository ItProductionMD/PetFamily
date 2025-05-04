using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.PetManagment.ChangeMainPetImage;
using PetFamily.Application.Commands.PetManagment.ChangePetsOrder;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetTypeManagment.Entities;
using PetFamily.Domain.PetTypeManagment.Root;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;

namespace PetFamily.IntegrationTests.PetFeatures;

public class ChangePetsOrderTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<ChangePetsOrderCommand>(factory)
{
    public Volunteer SeedVolunteer { get; set; } = null!;
    public Species SeedSpecies { get; set; } = null!;

    [Fact]
    public async Task Should_change_pets_order_correctly()
    {
        //ARRANGE
        var initialPetIdsOrder = SeedVolunteer.Pets
            .OrderBy(p => p.SerialNumber.Value)
            .Select(p => p.Id)
            .ToList();

        var newPetIdsOrder = initialPetIdsOrder
            .AsEnumerable()
            .Reverse()
            .ToList();

        var command = new ChangePetsOrderCommand(SeedVolunteer.Id, newPetIdsOrder);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        var updatedVolunteer = await _dbContext.Volunteers
            .AsNoTracking()
            .Include(v=>v.Pets)
            .SingleOrDefaultAsync();
        Assert.NotNull(updatedVolunteer);

        var updatedPetIdsOrder = updatedVolunteer.Pets
            .OrderBy(p=>p.SerialNumber.Value)
            .Select(p => p.Id)
            .ToList();

        Assert.True(newPetIdsOrder.SequenceEqual(updatedPetIdsOrder));
    }
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        SeedSpecies = new SpeciesTestBuilder()
            .WithBreeds(["breedOne", "breedTwo"]).Species;
        await Seeder.Seed(SeedSpecies, _dbContext);

        SeedVolunteer = new VolunteerTestBuilder()
            .WithPets(10, SeedSpecies).Volunteer;
        await Seeder.Seed(SeedVolunteer, _dbContext);
    }
}
