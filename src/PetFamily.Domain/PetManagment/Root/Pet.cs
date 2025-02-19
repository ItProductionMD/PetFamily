using PetFamily.Domain.PetAggregates.Enums;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using PetFamily.Domain.Shared.Validations;
using PetFamily.Domain.PetAggregates.ValueObjects;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.Dtos;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;

namespace PetFamily.Domain.PetAggregates.Root;

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
    public double Weight { get; private set; }
    public double Height { get; private set; }
    public string? Color { get; private set; }
    public PetType PetType { get; private set; } //Species and Breed
    public Phone? OwnerPhone { get; private set; }
    public IReadOnlyList<RequisitesInfo> DonateDetailsBox { get; private set; }
    public string? HealthInfo { get; private set; }
    public Address? Adress { get; private set; }
    public HelpStatus HelpStatus { get; private set; }
    public IReadOnlyList<Image> Images => _images;

    private readonly List<Image> _images = [];
    public Guid VolunteerId { get; private set; }//Navigation property

    private bool _isDeleted;

    private DateTime? _deletedDateTime;
    private Pet(Guid id) : base(id) { }//Ef core needs this

    private Pet(Guid id, PetDomainDto petDto) : base(id)
    {
        DateTimeCreated = DateTime.UtcNow;

        Name = petDto.Name!;

        DateOfBirth = petDto.DateOfBirth;

        Description = petDto.Description;

        IsVaccinated = GetBoolValue(petDto.IsVaccinated);

        IsSterilized = GetBoolValue(petDto.IsSterilized);

        Weight = petDto.Weight;

        Height = petDto.Height;

        Color = petDto.Color;

        PetType = petDto.PetType!;

        OwnerPhone = petDto.OwnerPhone;

        DonateDetailsBox = petDto.DonateDetails;

        HealthInfo = petDto.HealthInfo;

        Adress = petDto.Adress;

        HelpStatus = petDto.HelpStatus;

        _images = petDto.Images;
    }

    public static Result<Pet> Create(PetDomainDto petDto)
    {
        var validatePetDomain = Validate(petDto);
        if (validatePetDomain.IsFailure)
            return validatePetDomain;

        return Result.Ok(new Pet(Guid.NewGuid(), petDto));
    }

    public static UnitResult Validate(PetDomainDto petDto)
    {
        return UnitResult.ValidateCollection(
            () => ValidateNumber(petDto.Images.Count, "Images count", 0, 10),

            () => ValidateRequiredField(petDto.Name, "Pet name", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),

            () => ValidateRequiredObject(petDto.PetType, "Pet type"),

            () => ValidateNonRequiredField(petDto.Description, "Pet description", MAX_LENGTH_LONG_TEXT),

            () => ValidateNonRequiredField(petDto.HealthInfo, "Pet health", MAX_LENGTH_LONG_TEXT),

            () => ValidateNonRequiredField(petDto.Color, "Pet color", MAX_LENGTH_SHORT_TEXT));
    }

    //TODO: Add more validations

    public void SetSerialNumber(PetSerialNumber serialNumber)
    {
        SerialNumber = serialNumber;
    }
    public PetSerialNumber GetSerialNumber() => SerialNumber;
    private static bool GetBoolValue(bool? variable) => variable ?? false;

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
    public bool IsAddImagesCountPermited(int count) => MAX_IMAGES_COUNT > Images.Count + count;

    public bool AddImages(List<string> imageNames)
    {
        if (IsAddImagesCountPermited(imageNames.Count) == false)
            return false;

        var newImageList = imageNames.Select(i => Image.Create(i).Data!).ToList();
        _images.AddRange(newImageList);
        return true;
    }
    public void DeleteImages(List<Image> imagesToDelete)
    {
        if (imagesToDelete.Count == 0)
            return;

        var imagesToDeleteSet = new HashSet<Image>(imagesToDelete);

        var newImageList = Images
            .Where(img => !imagesToDeleteSet.Contains(img))
            .ToList();

        if (newImageList.Count != Images.Count)
        {
            _images.Clear();
            _images.AddRange(newImageList);
        }
    }
    public void SetVolunteerId(Guid volunteerId)
    {
        VolunteerId = volunteerId;
    }
}
