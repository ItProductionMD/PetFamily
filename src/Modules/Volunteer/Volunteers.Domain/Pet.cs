using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Domain.Enums;
using Volunteers.Domain.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;
using Image = PetFamily.SharedKernel.ValueObjects.Image;

namespace Volunteers.Domain;

public class Pet : SoftDeletable, IEntity<Guid>
{
    public Guid Id { get; private set; }
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
    private Pet() { }//Ef core needs this

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
        PetSerialNumber serialNumber)
    {
        Id = id;
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
        return UnitResult.FromValidationResults(

            () => ValidateRequiredField(name, "Pet name", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),

            () => ValidateNonRequiredField(description, "Pet description", MAX_LENGTH_LONG_TEXT),

            () => ValidateNonRequiredField(healthInfo, "Pet health", MAX_LENGTH_LONG_TEXT),

            () => ValidateNonRequiredField(color, "Pet color", MAX_LENGTH_SHORT_TEXT));
    }

    public PetSerialNumber GetSerialNumber() => SerialNumber;

    public void SetSerialNumber(PetSerialNumber serialNumber) => SerialNumber = serialNumber;

    public void ChangePetStatus(HelpStatus helpStatus) => HelpStatus = helpStatus;


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

    public UnitResult Update(UpdatePetInfo info)
    {
        var validatePetDomain = Validate(info.Name, info.Description, info.Color, info.HealthInfo);
        if (validatePetDomain.IsFailure)
            return validatePetDomain;

        Name = info.Name;
        DateOfBirth = info.DateOfBirth;
        Description = info.Description;
        IsVaccinated = info.IsVaccinated;
        IsSterilized = info.IsSterilized;
        Weight = info.Weight;
        Height = info.Height;
        Color = info.Color;
        PetType = info.PetType;
        OwnerPhone = info.OwnerPhone ?? Phone.CreateEmpty();
        Requisites = info.Requisites;
        HelpStatus = info.HelpStatus;
        Address = info.Address ?? Address.CreateEmpty();

        return UnitResult.Ok();
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

public record UpdatePetInfo(
        string Name,
        DateOnly? DateOfBirth,
        string? Description,
        bool IsVaccinated,
        bool IsSterilized,
        double? Weight,
        double? Height,
        string? Color,
        PetType PetType,
        Phone? OwnerPhone,
        IReadOnlyList<RequisitesInfo> Requisites,
        HelpStatus HelpStatus,
        string? HealthInfo,
        Address? Address);


