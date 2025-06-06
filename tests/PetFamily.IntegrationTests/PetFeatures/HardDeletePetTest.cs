﻿using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using PetSpecies.Domain;
using Volunteers.Application.Commands.PetManagement.DeletePet;
using Volunteers.Domain;

namespace PetFamily.IntegrationTests.PetFeatures;

public class HardDeletePetTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<HardDeletePetCommand>(factory)
{
    private Species _species = null!;
    private Volunteer _volunteer = null!;

    [Fact]
    public async Task Should_delete_pet_correctly()
    {
        //ARRANGE
        var petIdToDelete = _volunteer.Pets[0].Id;
        var command = new HardDeletePetCommand(_volunteer.Id, petIdToDelete);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        var updatedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(p => p.Pets)
            .FirstOrDefaultAsync();

        Assert.NotNull(updatedVolunteer);
        Assert.Single(updatedVolunteer.Pets);
        Assert.True(petIdToDelete != updatedVolunteer.Pets[0].Id);
        Assert.Equal(1, updatedVolunteer.Pets[0].SerialNumber.Value);
        Assert.DoesNotContain(updatedVolunteer.Pets, p => p.Id == petIdToDelete);
    }

    [Fact]
    public async Task Should_delete_pet_with_error()
    {
        //ARRANGE
        var petIdToDelete = Guid.NewGuid();
        var command = new HardDeletePetCommand(_volunteer.Id, petIdToDelete);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsFailure);

        var updatedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(p => p.Pets)
            .FirstOrDefaultAsync();

        Assert.NotNull(updatedVolunteer);
        Assert.Equal(2, updatedVolunteer.Pets.Count);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _species = new SpeciesTestBuilder()
            .WithBreeds(["testBreed"]).Species;
        await DbContextSeedExtensions.SeedAsync(_speciesDbContext, _species);

        _volunteer = new VolunteerTestBuilder()
            .WithPets(2, _species).Volunteer;
        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, _volunteer);
    }

}
