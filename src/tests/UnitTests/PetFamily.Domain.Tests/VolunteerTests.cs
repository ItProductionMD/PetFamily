using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using PetFamily.Domain.PetManagment.Enums;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Shared.ValueObjects;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Net;
using System.Xml.Linq;
using PetFamily.Domain.PetManagment.Root;

namespace TestPetFamilyDomain;

public class VolunteerTests
{
    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(2, 1, 2)]
    [InlineData(2, 1, 1)]
    [InlineData(2, 2, 1)]
    [InlineData(10, 1, 5)]
    [InlineData(21, 5, 11)]
    [InlineData(21, 11, 5)]
    [InlineData(210, 100, 99)]
    [InlineData(210, 99, 100)]
    [InlineData(355, 26, 27)]
    [InlineData(366, 366, 2)]
    public void MovePetSerialNumber_ShouldUpdatedCorrectly(
        int petsCount,
        int oldPosition,
        int newPosition)
    {
        // ARRANGE
        bool isMovedToRight = oldPosition < newPosition;
        bool isMovedToLeft = oldPosition > newPosition;

        var volunteer = TestDataFactory.CreateVolunteer(petsCount);

        var movedPet = volunteer.Pets
            .FirstOrDefault(p => p.GetSerialNumber().Value == oldPosition)
            ?? throw new TestCanceledException("Moved Pet Not Found!");

        var affectedPetsIndexes = new HashSet<int>();

        int firstAffectedIndex = Math.Min(oldPosition, newPosition) - 1;
        int lastAffectedIndex = Math.Max(oldPosition, newPosition) - 1;

        for (int i = firstAffectedIndex; i <= lastAffectedIndex; i++)
            affectedPetsIndexes.Add(i);

        var newSerialNumber = PetSerialNumber.Create(newPosition, volunteer).Data;
        // ACT
        volunteer.MovePetSerialNumber(movedPet, newSerialNumber!);

        // ASSERT
        Assert.Equal(newPosition, movedPet.GetSerialNumber().Value);

        for (int i = 0; i < petsCount; i++)
        {
            var currentPet = volunteer.Pets[i];

            if (currentPet == movedPet) continue;//already has been controled

            int expectedSerialNumber = i + 1;

            if (affectedPetsIndexes.Contains(i))
            {
                if (isMovedToLeft)
                    expectedSerialNumber += 1;//Increazing serialNumber to affected pets

                if (isMovedToRight)
                    expectedSerialNumber += -1;//Decreazing serialNumber to affected pets
            }
            Assert.Equal(expectedSerialNumber, currentPet.GetSerialNumber().Value);
        }
    }


    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    public void AddPet_ShouldAddPet(int petInitialCount)
    {
        //ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(petInitialCount);
        //ACT
        var pet = volunteer.CreateAndAddPet(
            "Lesy",
            null,
            null,
            true,
            true,
            null,
            null,
            null,
            PetType.Create(BreedID.NewGuid(), SpeciesID.NewGuid()).Data!,
            Phone.CreateNotEmpty("67777745222", "+373").Data!,
            [],
            HelpStatus.ForHelp,
            null,
            Address.CreatePossibleEmpty("", "Florence", "Barca", "11A").Data!);

        //ASSERT
        Assert.NotNull(pet);
        Assert.Equal(petInitialCount + 1, volunteer.Pets.Count);
        Assert.Equal(Guid.Empty, pet.Id);
        Assert.Equal(petInitialCount + 1, pet.GetSerialNumber().Value);
        Assert.Equal(1, volunteer.Pets[0].GetSerialNumber().Value);
    }


    [Theory]
    [InlineData("FirstName", "LastName", "aaa@gmail.com", "+373", "699656666", 1, "description")]
    [InlineData("FirstName", "LastName", "aaa@gmail.ru", "+33", "599656666", 0, "")]
    [InlineData("FName", "LName", "aaa@mail.com", "+373", "699656666", 1, null)]
    [InlineData("FirstName", "LastName", "a@gmail.com", "+373", "699656666", 1, "description description")]
    [InlineData("First-Name", "LastName", "gfwfeuyi@gmail.com", "+373", "6998764666", 1, "description ")]
    public void ValidateVolunteer_ShouldValidateOk(
        string? FirstName,
        string? LastName,
        string? email,
        string? phoneRegionCode,
        string? phoneNumber,
        int experienceYears,
        string? description)
    {
        //ACT
        var validationResult = Volunteer.Validate(
            FullName.Create(FirstName, LastName).Data,
            email,
            Phone.CreateNotEmpty(phoneNumber, phoneRegionCode).Data,
            experienceYears,
            description);

        //ASSERT
        Assert.True(validationResult.IsSuccess);
    }
    [Theory]
    [InlineData("", "LastName", "aaa@gmail.com", "+373", "699656666", 1, "description")]//FirstName
    [InlineData(null, "", "aaa@gmail.ru", "+33", "599656666", 0, "")]//FirstName
    [InlineData("aaaaaaaaaaaaaaagfhjhjjkhasnjknjknjknkjnknkjnkjnkjnkjnknknkjnkjnknjkn", "LName", "ag@mail.com", "+373", "699656666", 1, null)]//FirstName
    [InlineData("ihb6476gbjhb4", "LastName", "ag@mail.com", "+373", "699656666", 1, "description description")] //Firstname
    [InlineData("FirstName", "", "aaa@gmail.com", "+373", "699656666", 1, "description")]//LastName
    [InlineData("FirstName", null, "aaa@gmail.ru", "+33", "599656666", 0, "")]//LastName
    [InlineData("FName", "LName??", "aaa@mail.com", "+373", "699656666", 1, null)]//LastName
    [InlineData("FirstName", "LastName", null, "+373", "699656666", 1, "description description")]//Email
    [InlineData("First-Name", "LastName", "", "+373", "6998764666", 1, "description ")]//Email
    [InlineData("First-Name", "LastName", "gfwfeuyigmail.com", "+373", "6998764666", 1, "description ")]//Email
    [InlineData("First-Name", "LastName", "gfwfeuyi@gmailcom", "+373", "6998764666", 1, "description ")]//Email
    [InlineData("First-Name", "LastName", "gfwfeuyi@gmail.com", "", "6998764666", 1, "description ")]//PhoneRegionCode
    [InlineData("First-Name", "LastName", "gfwfeuyi@gmail.com", null, "6998764666", 1, "description ")]//PhoneRegionCode
    [InlineData("First-Name", "LastName", "gfwfeuyi@gmail.com", "-373", "6998764666", 1, "description ")]//PhoneRegionCode
    [InlineData("First-Name", "LastName", "gfwfeuyi@gmail.com", "+3754323", "6998764666", 1, "description ")]//PhoneRegionCode
    [InlineData("First-Name", "LastName", "gfwfeuyi@gmail.com", "+373", "", 1, "description ")]//PhoneNumber
    [InlineData("First-Name", "LastName", "gfwfeuyi@gmail.com", "+373", null, 1, "description ")]//PhoneNumber
    [InlineData("First-Name", "LastName", "gfwfeuyi@gmail.com", "+373", "6998764666A", 1, "description ")]//PhoneNumber
    [InlineData("First-Name", "LastName", "gfwfeuyi@gmail.com", "+373", "6998764666", 1000, "description ")]//ExperienceYears
    [InlineData("First-Name", "LastName", "gfwfeuyi@gmail.com", "+373", "6998764666", -11, "description ")]//ExperienceYears
    public void ValidateVolunteer_ShouldValidateWithError(
        string? FirstName,
        string? LastName,
        string? email,
        string? phoneRegionCode,
        string? phoneNumber,
        int experienceYears,
        string? description)
    {
        //ACT
        var validationResult = Volunteer.Validate(
            FullName.Create(FirstName, LastName).Data,
            email,
            Phone.CreateNotEmpty(phoneNumber, phoneRegionCode).Data,
            experienceYears,
            description);

        //ASSERT
        Assert.True(validationResult.IsFailure);
    }
    [Fact]
    public void DeleteVolunteer_ShouldSetVolunteerAndAllPetsAsDeleted()
    {
        //ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(10);
        //ACT
        volunteer.SoftDelete();
        //ASSERT
        Assert.True(volunteer.IsDeleted);
        Assert.All(volunteer.Pets, p => Assert.True(p.IsDeleted));
    }
    [Fact]
    public void RestoreVolunteer_ShouldSetVolunteerAndAllPetsAsRestored()
    {
        //ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(10);
        volunteer.SoftDelete();
        //ACT
        volunteer.Restore();
        //ASSERT
        Assert.False(volunteer.IsDeleted);
        Assert.All(volunteer.Pets, p => Assert.False(p.IsDeleted));
    }
    [Theory]
    [InlineData("FirstName", "LastName", "+373", "699656666", "aa@gmail.com", 1, "description")]
    [InlineData("FirstName", "LastName", "+39", "699656666", "ddh@mail.ru", 0, "description hiohoie ioho")]
    [InlineData("First-Name", "LastName", "+44", "6996562317", "fdgA2@inbox.ru", 10, "description iqheu 123 123y78- ")]

    public void UpdateMainInfo_ShouldUpdateOk(
        string FirstName,
        string LastName,
        string RegionCode,
        string PhoneNumber,
        string email,
        int experienceYears,
        string description)
    {
        //ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(10);
        var newFullName = FullName.Create(FirstName, LastName).Data!;
        var phone = Phone.CreateNotEmpty(PhoneNumber, RegionCode).Data!;
        //ACT
        var updateResult = volunteer.UpdateMainInfo(newFullName, email, phone, experienceYears, description);
        //ASSERT
        Assert.Equal(newFullName, volunteer.FullName);
        Assert.Equal(email, volunteer.Email);
        Assert.Equal(phone, volunteer.Phone);
        Assert.Equal(experienceYears, volunteer.ExperienceYears);
        Assert.Equal(description, volunteer.Description);
        Assert.True(updateResult.IsSuccess);
    }
    [Theory]
    [InlineData("", "LastName", "+373", "699656666", "aa@gmail.com", 1, "description")]//FirstName
    [InlineData("FirstName", "LastName", "", "699656666", "ddh@mail.ru", 0, "description hiohoie ioho")]//RegionCode
    [InlineData("First-Name", "LastName", "+44", "6996562317", "fdgA2inbox.ru", 10, "description iqheu 123 123y78- ")]//Email

    public void UpdateMainInfo_ShouldUpdateWithErrors(
        string FirstName,
        string LastName,
        string RegionCode,
        string PhoneNumber,
        string email,
        int experienceYears,
        string description)
    {
        //ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(10);
        var newFullName = FullName.Create(FirstName, LastName).Data!;
        var phone = Phone.CreateNotEmpty(PhoneNumber, RegionCode).Data!;
        //ACT
        var updateResult = volunteer.UpdateMainInfo(newFullName, email, phone, experienceYears, description);
        //ASSERT
        Assert.False(updateResult.IsSuccess);
    }

    [Theory]
    [InlineData(10, 5)]
    [InlineData(13, 1)]
    [InlineData(5, 5)]
    [InlineData(1, 1)]
    public void ShouldSoftDeletePetCorrectly(int petsCount, int serialNumberOfPet)
    {
        // ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(petsCount);
        var pet = volunteer.Pets.FirstOrDefault(p => p.SerialNumber.Value == serialNumberOfPet);
        if (pet == null)
            throw new TestCanceledException("Pet Not Found!");
        var existingPetsCountAfterDelete = petsCount - 1;
        // ACT
        volunteer.SoftDeletePet(pet);

        // ASSERT
        Assert.True(pet.IsDeleted);
        Assert.NotNull(pet.DeletedDateTime);
        Assert.Equal(PetSerialNumber.Empty(), pet.SerialNumber);
        Assert.Equal(existingPetsCountAfterDelete, volunteer.ExistingPetsCount);
        Assert.Equal(petsCount, volunteer.Pets.Count);
        if (petsCount != serialNumberOfPet)
        {
            var petWithNewSerialNumber = volunteer.Pets
                .Where(p => p.IsDeleted == false)
                .FirstOrDefault(p => p.SerialNumber.Value == serialNumberOfPet);

            Assert.NotNull(pet);
        }
    }

    [Theory]
    [InlineData(10, 5)]
    [InlineData(13, 1)]
    [InlineData(5, 5)]
    [InlineData(1, 1)]
    public void ShouldHardDeletePetCorrectly(int petsCount, int serialNumberOfPet)
    {
        // ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(petsCount);
        var pet = volunteer.Pets.FirstOrDefault(p => p.SerialNumber.Value == serialNumberOfPet);
        if (pet == null)
            throw new TestCanceledException("Pet Not Found!");

        var totalPetsCountAfterDelete = petsCount - 1;
        // ACT
        volunteer.HardDeletePet(pet);

        // ASSERT
        Assert.Equal(totalPetsCountAfterDelete, volunteer.Pets.Count);

        if (petsCount != serialNumberOfPet)
        {
            var petWithNewSerialNumber = volunteer.Pets
                .FirstOrDefault(p => p.SerialNumber.Value == serialNumberOfPet);

            Assert.NotNull(pet);
        }
    }

    [Theory]
    [InlineData(10, 5)]
    [InlineData(13, 1)]
    public void ShouldRestorePetCorrectly(int petsCount, int serialNumberOfPet)
    {
        // ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(petsCount);
        var pet = volunteer.Pets.FirstOrDefault(p => p.SerialNumber.Value == serialNumberOfPet);
        if (pet == null)
            throw new TestCanceledException("Pet Not Found!");
        volunteer.SoftDeletePet(pet);
        // ACT
        var result = volunteer.RestorePet(pet.Id);
        // ASSERT
        Assert.True(result.IsSuccess);
        Assert.False(pet.IsDeleted);
        Assert.Null(pet.DeletedDateTime);
        Assert.Equal(petsCount, volunteer.Pets.Count);
        Assert.True(pet.SerialNumber.Value == volunteer.ExistingPetsCount);
    }


}