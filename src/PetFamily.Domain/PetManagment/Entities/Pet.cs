using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.Shared.Validations;
using PetFamily.Domain.Results;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.Enums;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using System.Collections.Generic;
using PetFamily.Domain.Shared.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using Image = PetFamily.Domain.Shared.ValueObjects.Image;
using PetFamily.Domain.DomainError;

namespace PetFamily.Domain.PetManagment.Entities;

public class Pet : Entity<Guid>, ISoftDeletable
{
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
    private List<Image> _images = [];
    private bool _isDeleted;
    public bool IsDeleted => _isDeleted;
    private DateTime? _deletedDateTime;
    public DateTime? DeletedDateTime => _deletedDateTime;
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
        Phone? ownerPhone,
        IReadOnlyList<RequisitesInfo> requisites,
        HelpStatus helpStatus,
        string? healthInfo,
        Address? address,
        PetSerialNumber serialNumber)
    {
        var validatePetDomain = Validate(name, description, color, healthInfo);
        return validatePetDomain.IsFailure
            ? validatePetDomain
            : Result.Ok(new Pet(
                Guid.Empty,
                name,
                dateOfBirth,
                description,
                isVaccinated,
                isSterilized,
                weight,
                height,
                color,
                petType,
                ownerPhone ?? Phone.CreateEmpty(),
                requisites,
                helpStatus,
                healthInfo,
                address ?? Address.CreateEmpty(),
                serialNumber));
    }
    public static UnitResult Validate(
        string name,
        string? description,
        string? color,
        string? healthInfo)
    {
        return UnitResult.ValidateCollection(

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
    public void SetAsDeleted()
    {
        _isDeleted = true;
        _deletedDateTime = DateTime.UtcNow;
    }
    public void Restore()
    {
        _isDeleted = false;
        _deletedDateTime = null;
    }

    public void AddImages(List<string> imageNames)
    {
        foreach (string imageName in imageNames)
        {
            var image = Image.Create(imageName).Data!;
            _images.Add(image);
        }
    }
    public List<string> DeleteImages(List<string> imageNames)
    {
        var imagesToDeleteSet = new HashSet<string>(imageNames); //delete reapeted images
        var deletedImages = new List<string>();

        _images.RemoveAll(image =>
        {
            if (imagesToDeleteSet.Contains(image.Name))
            {
                deletedImages.Add(image.Name);
                return true;
            }
            return false;
        });

        return deletedImages;
    }
    public List<string> DeleteImages(List<Image> images)
    {
        if (images.Count == 0)
            return [];

        var imagesToDeleteSet = new HashSet<string>(images.Select(i => i.Name)); //delete reapeted images
        var deletedImages = new List<string>();

        _images.RemoveAll(image =>
        {
            if (imagesToDeleteSet.Contains(image.Name))
            {
                deletedImages.Add(image.Name);
                return true;
            }
            return false;
        });

        return deletedImages;
    }
    public void ChangePetStatus(HelpStatus helpStatus)
    {
        HelpStatus = helpStatus;
    }

    public UnitResult Update(
        string name,
        DateOnly? dateOfBirth,
        string? description,
        bool isVaccinated,
        bool isSterilized,
        double? weight,
        double? height,
        string? color,
        PetType petType,
        Phone? ownerPhone,
        IReadOnlyList<RequisitesInfo> requisites,
        HelpStatus helpStatus,
        string? healthInfo,
        Address? address)
    {
        var validatePetDomain = Validate(name, description, color, healthInfo);
        if (validatePetDomain.IsFailure)
            return validatePetDomain;

        Name = name;
        DateOfBirth = dateOfBirth;
        Description = description;
        IsVaccinated = isVaccinated;
        IsSterilized = isSterilized;
        Weight = weight;
        Height = height;
        Color = color;
        PetType = petType;
        OwnerPhone = ownerPhone ?? Phone.CreateEmpty();
        Requisites = requisites;
        HelpStatus = helpStatus;
        Address = address ?? Address.CreateEmpty();

        return UnitResult.Ok();
    }

    public void UpdateHelpStatus(HelpStatus helpStatus)
    {
        HelpStatus = helpStatus;
    }

    public UnitResult ChangeMainPhoto(string imageName)
    {
        if (!_images.Any(i => i.Name == imageName))
            return UnitResult.Fail(Error.NotFound($"Image with name: {imageName}"));

        _images = _images
            .OrderByDescending(i => i.Name == imageName)
            .ToList();

        return UnitResult.Ok();
    }
}

