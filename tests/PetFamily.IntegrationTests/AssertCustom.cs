﻿using Volunteers.Application.Commands.PetManagement.AddPet;
using Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;
using Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;
using Volunteers.Domain;

namespace PetFamily.IntegrationTests;

public static class AssertCustom
{
    public static void AreEqualData(CreateVolunteerCommand command, Volunteer? volunteer)
    {
        var commandRequisites = command.Requisites.ToList();

        Assert.NotNull(volunteer);
        Assert.NotEqual(Guid.Empty, volunteer.Id);
        Assert.Equal(command.FirstName, volunteer.FullName.FirstName);
        Assert.Equal(command.LastName, volunteer.FullName.LastName);
        Assert.Equal(command.Description, volunteer.Description);
        Assert.Equal(command.ExperienceYears, volunteer.ExperienceYears);

        Assert.Equal(commandRequisites.Count, volunteer.Requisites.Count);
        for (int i = 0; i < volunteer.Requisites.Count; i++)
        {
            Assert.Equal(commandRequisites[i].Name, volunteer.Requisites[i].Name);
            Assert.Equal(commandRequisites[i].Description, volunteer.Requisites[i].Description);
        }
    }

    public static void AreEqualData(UpdateVolunteerCommand command, Volunteer? volunteer)
    {

        Assert.NotNull(volunteer);
        Assert.NotEqual(Guid.Empty, volunteer.Id);
        Assert.Equal(command.FirstName, volunteer.FullName.FirstName);
        Assert.Equal(command.LastName, volunteer.FullName.LastName);
        Assert.Equal(command.Description, volunteer.Description);
        Assert.Equal(command.ExperienceYears, volunteer.ExperienceYears);
    }

    public static void AreEqualData(AddPetCommand command, Pet pet)
    {
        Assert.Equal(command.PetName, pet.Name);
        Assert.Equal(command.Description, pet.Description);
        Assert.Equal(command.Color, pet.Color);
        Assert.Equal(command.DateOfBirth, pet.DateOfBirth);
        Assert.Equal(command.HomeNumber, pet.Address.Number);
        Assert.Equal(command.Region, pet.Address.Region);
        Assert.Equal(command.City, pet.Address.City);
        Assert.Equal(command.BreedId, pet.PetType.BreedId);
        Assert.Equal(command.SpeciesId, pet.PetType.SpeciesId);
        Assert.Equal(command.OwnerPhoneNumber, pet.OwnerPhone.Number);
        Assert.Equal(command.OwnerPhoneRegion, pet.OwnerPhone.RegionCode);
        Assert.Equal(command.HealthInfo, pet.HealthInfo);
        Assert.Equal(command.HelpStatus, ((int)pet.HelpStatus));
        Assert.Equal(command.Height, pet.Height);
        Assert.Equal(command.Weight, pet.Weight);
        Assert.Equal(command.IsSterilized, pet.IsSterilized);
        Assert.Equal(command.IsVaccinated, pet.IsVaccinated);

        var commandRequisitesList = command.Requisites.ToList();

        Assert.Equal(commandRequisitesList.Count, pet.Requisites.Count);
        for (int i = 0; i < commandRequisitesList.Count; i++)
        {
            Assert.Equal(commandRequisitesList[i].Name, pet.Requisites[i].Name);
            Assert.Equal(commandRequisitesList[i].Description, pet.Requisites[i].Description);
        }
    }
}
