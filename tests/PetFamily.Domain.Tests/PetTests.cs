﻿using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;

namespace TestPetFamilyDomain;

public class PetTests
{
    [Theory]
    [InlineData("Tom", "Cute cat", "White","Healthy")]
    [InlineData("Jerry",null, "White", "Healthy")]
    [InlineData("Jerry", "Cute cat", null, "Healthy")]
    [InlineData("Max","Cute cat", "White",null)]
    [InlineData("Jerry", null,null,null)]

    public void ValidatePets_ShouldValidatePetData_Ok(
        string name,
        string? description,
        string? color,
        string? healthInfo)
    {
        //ACT
        var validateResult = Pet.Validate(
            name,
            description,
            color,
            healthInfo);
        //ASSERT
        Assert.True(validateResult.IsSuccess);
    }

    [Theory]
    [InlineData("Name", MAX_LENGTH_SHORT_TEXT + 1, 0, 0, 0)] // name too long
    [InlineData("Description", MAX_LENGTH_SHORT_TEXT, MAX_LENGTH_LONG_TEXT + 1, 0, 0)] // description too long
    [InlineData("Color", MAX_LENGTH_SHORT_TEXT, 0, MAX_LENGTH_SHORT_TEXT + 1, 0)] // color too long
    [InlineData("HealthInfo", MAX_LENGTH_SHORT_TEXT, 0, 0, MAX_LENGTH_LONG_TEXT + 1)] // healthInfo too long
    public void ValidatePets_ShouldFail_WhenStringFieldLengthIsTooLong(
    string fieldNameWithError,
    int nameSize,
    int descriptionSize,
    int colorSize,
    int healthInfoSize)
    {
        // ARRANGE
        var name = new string('a', nameSize);
        var description = new string('a', descriptionSize);
        var color = new string('a', colorSize);
        var healthInfo = new string('a', healthInfoSize);
        // ACT
        var validateResult = Pet.Validate(name, description, color, healthInfo);
        // ASSERT
        Assert.False(validateResult.IsSuccess);
        Assert.Single(validateResult.Errors);
        Assert.Equal(Error.InvalidLength(fieldNameWithError).Type,validateResult.Errors.FirstOrDefault()!.Type);
    }

    [Theory]
    [InlineData(null)] // name is null
    [InlineData("")] // name is empty
    public void ValidatePets_ShouldFail_WhenNameIsNullOrEmpty(string? name)
    {
        // ARRANGE
        var description = "cute cat";
        var color = "black";
        var healthInfo = "good health check";

        // ACT
        var validateResult = Pet.Validate(name, description, color, healthInfo);

        // ASSERT
        Assert.False(validateResult.IsSuccess);
        Assert.Single(validateResult.Errors);

        var error = validateResult.Errors.First();
        Assert.Equal(Error.InvalidFormat("name").Type, error.Type);
    }

    [Theory]
    [InlineData("ubwuec12")] 
    [InlineData("gvrjh!")] 
    [InlineData("?ec")] 
    [InlineData("wepc.")] 
    public void ValidatePets_ShouldFail_WhenNamePatternIsNotCorresponding(string name)
    {
        // ACT
        var validateResult = Pet.Validate(name,null,null,null);

        // ASSERT
        Assert.False(validateResult.IsSuccess);
        Assert.Single(validateResult.Errors);

        var error = validateResult.Errors.First();
        Assert.Equal(Error.InvalidFormat("name").Type, error.Type);
    }

    [Fact]
    public void DeletePet_ShouldMarkAsDeleted()
    {
        // ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(1);
        var pet =volunteer.Pets[0];
        // ACT
        pet.Delete();
        // ASSERT
        Assert.True(pet.IsDeleted);
        Assert.NotNull(pet.DeletedDateTime);
    }

    [Fact]
    public void Restore_ShouldMarkAsNotDeleted()
    {
        // ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(1);
        var pet = volunteer.Pets[0];
        pet.Delete();
        //ACT
        pet.Restore();
        // ASSERT
        Assert.False(pet.IsDeleted);
        Assert.Null(pet.DeletedDateTime);
    }

    [Fact]
    public void AddImages_ShouldAddImagesToPetCorrectly()
    {
        // ARRANGE
        Volunteer volunteer = TestDataFactory.CreateVolunteer(1);
        var pet = volunteer.Pets[0];
        var images = new List<Image>
        {
            Image.Create(Guid.NewGuid().ToString()).Data!,
            Image.Create(Guid.NewGuid().ToString()).Data!
        };
        // ACT
        var addedImages = pet.AddImages(images);
        // ASSERT
        Assert.Equal(2, addedImages.Count);
    }

    [Fact]
    public void DeleteImages_ShouldDeleteALLImagesCorrectly()
    {
        //ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(1);
        var pet = volunteer.Pets[0];
        var imageName1 = Guid.NewGuid().ToString();
        var imageName2 = Guid.NewGuid().ToString();
        var images = new List<Image>
        {
            Image.Create(imageName1).Data!,
            Image.Create(imageName2).Data!
        };
        pet.AddImages(images);
        //ACT
        var deleteImages = pet.DeleteImages(images);
        //ASSERT
        Assert.Equal(imageName2, deleteImages[1]);
        Assert.Equal(imageName1, deleteImages[0]);
        Assert.Equal(0, pet.Images.Count);
    }

    [Fact]
    public void DeleteImages_ShouldDeleteOneImageCorrectly()
    {
        //ARRANGE
        var volunteer = TestDataFactory.CreateVolunteer(1);
        var pet = volunteer.Pets[0];
        var imageName1 = Guid.NewGuid().ToString();
        var imageName2 = Guid.NewGuid().ToString();
        var imageName3 = Guid.NewGuid().ToString();
        var images = new List<Image>
        {
            Image.Create(imageName1).Data!,
            Image.Create(imageName2).Data!,
            Image.Create(imageName3).Data!
        };
        pet.AddImages(images);
        //ACT
        var deleteImages = pet.DeleteImages([images[2]]);
        //ASSERT
        Assert.Equal(deleteImages[0], images[2].Name);
        Assert.Equal(2, pet.Images.Count);
    }
}
