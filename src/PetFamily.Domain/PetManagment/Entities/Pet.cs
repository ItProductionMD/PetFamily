﻿using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.Shared.Validations;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.Dtos;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.Enums;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using System.Collections.Generic;

namespace PetFamily.Domain.PetManagment.Entities;

public class Pet : Entity<Guid>, ISoftDeletable
{
    public const int MAX_IMAGES_COUNT = 10;
    public string Name { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public DateTime DateTimeCreated { get; private set; }
    public string? Description { get; private set; }
    public PetSerialNumber SerialNumber { get; private set; }
    public bool IsVaccinated { get; private set; }
    public bool IsSterilized { get; private set; }
    public double? Weight { get; private set; }
    public double? Height { get; private set; }
    public string? Color { get; private set; }
    public PetType PetType { get; private set; } //Species and Breed
    public Phone OwnerPhone { get; private set; }
    public IReadOnlyList<RequisitesInfo> Requisites { get; private set; }
    public string? HealthInfo { get; private set; }
    public Address Address { get; private set; }
    public HelpStatus HelpStatus { get; private set; }
    public IReadOnlyList<Image> Images => _images;
    private readonly List<Image> _images = [];
    private bool _isDeleted;
    private DateTime? _deletedDateTime;
    private Pet(Guid id) : base(id) { }//Ef core needs this

    private Pet(
        Guid id,
        string name,
        DateOnly? dateOfBirth,
        string? description,
        bool isVaccinated,
        bool isSterilized,
        double? weight,
        double? height,
        string? color,
        PetType petType,
        Phone ownerPhone,
        IReadOnlyList<RequisitesInfo> requisites,
        List<Image> images,
        HelpStatus helpStatus,
        string? healthInfo,
        Address address,
        PetSerialNumber serialNumber) : base(id)
    {
        DateTimeCreated = DateTime.UtcNow;
        Name = name;
        DateOfBirth = dateOfBirth;
        Description = description;
        IsVaccinated = isVaccinated;
        IsSterilized = isSterilized;
        Weight = weight;
        Height = height;
        Color = color;
        PetType = petType;
        OwnerPhone = ownerPhone;
        Requisites = requisites;
        HealthInfo = healthInfo;
        Address = address;
        HelpStatus = helpStatus;
        _images = images;
        SerialNumber = serialNumber;
    }

    public static Result<Pet> Create(
        string name,
        DateOnly? dateOfBirth,
        string? description,
        bool isVaccinated,
        bool isSterilized,
        double? weight,
        double? height,
        string? color,
        PetType petType,
        Phone ownerPhone,
        IReadOnlyList<RequisitesInfo> requisites,
        List<Image> images,
        HelpStatus helpStatus,
        string? healthInfo,
        Address address,
        PetSerialNumber serialNumber)
    {
        var validatePetDomain = Validate(name, dateOfBirth, description, color, ownerPhone, images, healthInfo);

        return validatePetDomain.IsFailure
            ? validatePetDomain
            : Result.Ok(new Pet(
                Guid.NewGuid(),
                name,
                dateOfBirth,
                description,
                isVaccinated,
                isSterilized,
                weight,
                height,
                color,
                petType,
                ownerPhone,
                requisites,
                images,
                helpStatus,
                healthInfo,
                address,
                serialNumber));
    }

    public static UnitResult Validate(
        string name,
        DateOnly? dateOfBirth,
        string? description,
        string? color,
        Phone ownerPhone,
        List<Image> images,
        string? healthInfo)
    {
        return UnitResult.ValidateCollection(

            () => ValidateNumber(images.Count, "Images count", 0, 10),

            () => ValidateRequiredField(name, "Pet name", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),

            () => ValidateNonRequiredField(description, "Pet description", MAX_LENGTH_LONG_TEXT),

            () => ValidateNonRequiredField(healthInfo, "Pet health", MAX_LENGTH_LONG_TEXT),

            () => ValidateNonRequiredField(color, "Pet color", MAX_LENGTH_SHORT_TEXT));
    }

    public void SetSerialNumber(PetSerialNumber serialNumber)
    {
        SerialNumber = serialNumber;
    }
    public PetSerialNumber GetSerialNumber() => SerialNumber;

    public void Delete()
    {
        _isDeleted = true;
        _deletedDateTime = DateTime.UtcNow;
    }
    public void Restore()
    {
        _isDeleted = false;
        _deletedDateTime = null;
    }
    public bool UpdateImages(List<string> imagesToDelete, List<string> imagesToAdd)
    {
        DeleteImages(imagesToDelete);
        return AddImages(imagesToAdd) == false ? false : true;
    }

    public bool AddImages(List<string> imageNames)
    {
        if (Images.Count + imageNames.Count > MAX_IMAGES_COUNT)
            return false;

        var newImageList = imageNames.Select(i => Image.Create(i).Data!).ToList();
        _images.AddRange(newImageList);
        return true;
    }
    public void DeleteImages(List<string> imagesToDelete)
    {
        if (imagesToDelete.Count == 0)
            return;

        var imagesToDeleteSet = new HashSet<string>(imagesToDelete);
        _images.RemoveAll(i => imagesToDeleteSet.Contains(i.Name));
    }
}
