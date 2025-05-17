using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Commands.PetManagment.AddPet;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;

namespace PetFamily.IntegrationTests.PetFeatures;

public class AddPetTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<AddPetResponse, AddPetCommand>(factory)
{

    [Fact]
    public async Task Should_add_pet_correctly()
    {
        //ARRANGE
        var expectedSerialNumber = 1;

        var seedSpecies = new SpeciesTestBuilder()
            .WithBreeds(["testBreed"]).Species;
        await Seeder.Seed(seedSpecies, _dbContext);

        var seedVolunteer = new VolunteerTestBuilder().Volunteer;
        await Seeder.Seed(seedVolunteer, _dbContext);

        var command = new AddPetCommand(
            VolunteerId: seedVolunteer.Id,
            PetName: "TestName",
            DateOfBirth: null,
            Description: "test description",
            IsVaccinated: true,
            IsSterilized: true,
            Weight: 1,
            Height: 1,
            Color: "testColor",
            SpeciesId: seedSpecies.Id,
            BreedId: seedSpecies.Breeds[0].Id,
            OwnerPhoneRegion: "+373",
            OwnerPhoneNumber: "69961150",
            HealthInfo: "testHealthInfo",
            HelpStatus: 1,
            City: "City",
            Region: "Region",
            Street: "Street",
            HomeNumber: "11",
            Requisites: [new("testRequisites", "test requisites description")]
            );
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.True(result.IsSuccess);

        var updatedVolunteer = await _dbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .SingleOrDefaultAsync();
        Assert.NotNull(updatedVolunteer);

        var addedPet = updatedVolunteer!.Pets.SingleOrDefault<Pet>();
        Assert.NotNull(addedPet);

        Assert.Equal(expectedSerialNumber, addedPet.SerialNumber.Value);
        Assert.Equal(result.Data!.PetId, addedPet.Id);
        Assert.True(addedPet.Id != Guid.Empty);
        AssertCustom.AreEqualData(command, addedPet);
    }
}
