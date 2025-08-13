using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.Fixtures;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using PetSpecies.Domain;
using Volunteers.Application.Commands.PetManagement.ChangeMainPetImage;
using Volunteers.Domain;

namespace PetFamily.IntegrationTests.PetFeatures;

public class ChangePetMainImageTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<ChangePetMainImageCommand>(factory)
{
    public Volunteer SeedVolunteer { get; set; } = null!;
    public Species SeedSpecies { get; set; } = null!;
    public const string FIRST_IMAGE = "first_image.png";
    public const string SECOND_IMAGE = "second_image.png";

    [Fact]
    public async Task Should_change_pet_image_correctly()
    {
        //ARRANGE
        var testedPet = SeedVolunteer.Pets[0];
        var initialMainPhoto = testedPet.Images[0].Name;
        var newMainPhoto = SECOND_IMAGE;
        var command = new ChangePetMainImageCommand(SeedVolunteer.Id, testedPet.Id, newMainPhoto);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(FIRST_IMAGE, initialMainPhoto);

        var updatedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .SingleOrDefaultAsync();

        Assert.NotNull(updatedVolunteer);

        var updatedPet = updatedVolunteer.GetPet(testedPet.Id);
        Assert.NotNull(updatedPet);
        Assert.Equal(newMainPhoto, updatedPet.Images[0].Name);
    }

    [Fact]
    public async Task Should_change_pet_image_with_the_same_name_correctly()
    {
        //ARRANGE
        var testedPet = SeedVolunteer.Pets[0];
        var initialMainPhoto = testedPet.Images[0].Name;

        var command = new ChangePetMainImageCommand(SeedVolunteer.Id, testedPet.Id, initialMainPhoto);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(FIRST_IMAGE, initialMainPhoto);

        var updatedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .SingleOrDefaultAsync();
        Assert.NotNull(updatedVolunteer);

        var updatedPet = updatedVolunteer.GetPet(testedPet.Id);
        Assert.NotNull(updatedPet);
        Assert.Equal(initialMainPhoto, updatedPet.Images[0].Name);
    }

    [Fact]
    public async Task Should_change_pet_image_with_failure_when_the_image_name_not_found()
    {
        //ARRANGE
        var testedPet = SeedVolunteer.Pets[0];
        var initialMainPhoto = testedPet.Images[0].Name;

        var command = new ChangePetMainImageCommand(SeedVolunteer.Id, testedPet.Id, "not_found_image.png");
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(FIRST_IMAGE, initialMainPhoto);

        var updatedVolunteer = await _volunteerDbContext.Volunteers.SingleOrDefaultAsync();
        Assert.NotNull(updatedVolunteer);

        var updatedPet = updatedVolunteer.Pets[0];
        Assert.NotNull(updatedPet);
        Assert.Equal(FIRST_IMAGE, updatedPet.Images[0].Name);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        SeedSpecies = new SpeciesTestBuilder()
            .WithBreeds(["testBreed"]).Species;

        await DbContextSeedExtensions.SeedAsync(_speciesDbContext, SeedSpecies);

        SeedVolunteer = new VolunteerTestBuilder()
            .WithPets(1, SeedSpecies).Volunteer;

        SeedVolunteer.Pets[0].AddImages([FIRST_IMAGE, SECOND_IMAGE]);

        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, SeedVolunteer);
    }
}
