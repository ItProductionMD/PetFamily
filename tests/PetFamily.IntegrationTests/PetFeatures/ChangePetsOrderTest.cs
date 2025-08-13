using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.Fixtures;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using PetSpecies.Domain;
using Volunteers.Application.Commands.PetManagement.ChangePetsOrder;
using Volunteers.Domain;

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

        var updatedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .SingleOrDefaultAsync();
        Assert.NotNull(updatedVolunteer);

        var updatedPetIdsOrder = updatedVolunteer.Pets
            .OrderBy(p => p.SerialNumber.Value)
            .Select(p => p.Id)
            .ToList();

        Assert.True(newPetIdsOrder.SequenceEqual(updatedPetIdsOrder));
    }
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        SeedSpecies = new SpeciesTestBuilder()
            .WithBreeds(["breedOne", "breedTwo"]).Species;
        await DbContextSeedExtensions.SeedAsync(_speciesDbContext, SeedSpecies);

        SeedVolunteer = new VolunteerTestBuilder()
            .WithPets(10, SeedSpecies).Volunteer;
        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, SeedVolunteer);
    }
}
