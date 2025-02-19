using PetFamily.Domain.PetAggregates.Enums;
using PetFamily.Domain.PetAggregates.Root;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.ValueObjects;
using PetFamily.Domain.Shared.Validations;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using PetFamily.Domain.PetAggregates.ValueObjects;
using PetFamily.Domain.Results;

namespace PetFamily.Domain.VolunteerAggregates.Root;

public class Volunteer : Entity<Guid>, ISoftDeletable
{
    public FullName FullName { get; private set; }
    public string Email { get; private set; }
    public Phone PhoneNumber { get; private set; }
    public int ExperienceYears { get; private set; }
    public string? Description { get; private set; }

    public IReadOnlyList<RequisitesInfo> Requisites { get; private set; }
    public IReadOnlyList<SocialNetworkInfo> SocialNetworks { get; private set; }
    public IReadOnlyList<Pet> Pets => _pets;
    private readonly List<Pet> _pets = [];

    private bool _isDeleted;
    private DateTime? _deletedDateTime;
    private Volunteer(Guid id) : base(id) { } //Ef core needs this

    private Volunteer(
        Guid id,
        FullName fullName,
        string email,
        Phone phoneNumber,
        int experienceYears,
        string? description,
        List<RequisitesInfo> requisites,
        List<SocialNetworkInfo> socialNetworks
        ) : base(id)
    {
        FullName = fullName;
        Email = email.Trim();
        PhoneNumber = phoneNumber;
        ExperienceYears = experienceYears;
        Description = description?.Trim();
        Requisites = requisites; // List Can be empty
        SocialNetworks = socialNetworks;//list can be emty
    }
    //--------------------------------------Factory method----------------------------------------//
    public static Result<Volunteer> Create(
        VolunteerID id,
        FullName fullName,
        string email,
        Phone phoneNumber,
        int expirienceYears,
        string? description,
        List<RequisitesInfo> requisites,
        List<SocialNetworkInfo> socialNetworkList
        )
    {
        var validationResult = Validate(fullName, email, phoneNumber, expirienceYears, description);

        return validationResult.IsFailure
            ? validationResult
            : Result.Ok(new Volunteer(
                id.Value, fullName!,
                email!, phoneNumber!,
                expirienceYears,
                description,
                requisites,
                socialNetworkList));
    }

    public static UnitResult Validate(
        FullName? fullName,
        string? email,
        Phone? phone,
        int experienceYears,
        string? description)
    {
        return UnitResult.ValidateCollection(
            () => ValidateRequiredObject<FullName>(fullName, "Volunteer fullName"),
            () => ValidateRequiredObject<Phone>(phone, "volunteer phone"),
            () => ValidateIntegerNumber(experienceYears, "volunteer experienece", 0, 100),
            () => ValidateRequiredField(email, "Volunteer email", MAX_LENGTH_SHORT_TEXT, EMAIL_PATTERN),
            () => ValidateNonRequiredField(description, "Volunteer description", MAX_LENGTH_LONG_TEXT));
    }
    //-------------------------------------Move Pet position--------------------------------------//
    public void MovePetSerialNumber(Pet movedPet, PetSerialNumber newSerialNumber)
    {
        var oldPosition = movedPet.GetSerialNumber().Value;
        var newPosition = newSerialNumber.Value;
        if (oldPosition == newPosition)
            return;

        var affectedPets = _pets.Where(p =>
            oldPosition > newPosition
                ? p.SerialNumber.Value >= newPosition && p.SerialNumber.Value < oldPosition
                : p.SerialNumber.Value <= newPosition && p.SerialNumber.Value > oldPosition);

        foreach (var pet in affectedPets)
        {
            var oldSerialNumberValue = pet.SerialNumber.Value;

            var updatedSerialNumberValue = oldPosition > newPosition
                ? oldSerialNumberValue + 1
                : oldSerialNumberValue - 1;

            var updatedSerialNumber = PetSerialNumber.Create(updatedSerialNumberValue, this);

            pet.SetSerialNumber(updatedSerialNumber.Data!);
        }

        movedPet.SetSerialNumber(newSerialNumber);
    }
    //--------------------------------------Add Pet-----------------------------------------------//
    public void AddPet(Pet pet)
    {
        pet.SetVolunteerId(Id);
        var petsCount = GetPetsCount();
        var serialNumberValue = petsCount + 1;
        _pets.Add(pet);
        var serialNumber = PetSerialNumber.Create(serialNumberValue, this).Data;
        pet.SetSerialNumber(serialNumber);
    }
    //--------------------------------------Remove Pet--------------------------------------------//
    public void RemovePet(Pet pet)
    {
        var maxSerialNumberValue = _pets.Max(pet => pet.SerialNumber.Value);

        var maxSerialNumber = PetSerialNumber.Create(maxSerialNumberValue, this);

        MovePetSerialNumber(pet, maxSerialNumber.Data);

        _pets.Remove(pet);
    }
    //--------------------------------------Get pets count----------------------------------------//
    public int GetPetsCount() => _pets.Count;
    //-----------------------------------Get Count of Pets for Adopt------------------------------//
    public int GetCountOfPetsForAdopt() =>
        _pets.Where(p => p.HelpStatus == HelpStatus.ForAdopt).Count();
    //----------------------------------Get Count of Pets for other Help--------------------------//
    public int GetCountOfPetsForHelp() =>
        _pets.Where(p => p.HelpStatus == HelpStatus.ForHelp).Count();
    //----------------------------------Get Count of Pets Adopted---------------------------------//
    public int GetCountOfPetsAdopted() =>
        _pets.Where(p => p.HelpStatus == HelpStatus.Adopted).Count();
    //------------------------------Set is Deleted fal se(for soft deleting)-----------------------//
    public void Delete()
    {
        _isDeleted = true;
        _deletedDateTime = DateTime.UtcNow;

        foreach (var pet in _pets)
            pet.Delete();
    }
    //------------------------------Set is Deleted true(for soft deleting)-----------------------//
    public void Restore()
    {
        _isDeleted = false;
        _deletedDateTime = null;

        foreach (var pet in _pets)
            pet.Restore();
    }
    //------------------------------------Update Main Info----------------------------------------//
    public bool UpdateMainInfo(
        FullName fullName,
        string email,
        Phone phoneNumber,
        int experienceYears,
        string description
        )
    {
        var validationResult = Validate(fullName, email, phoneNumber, experienceYears, description);
        if (validationResult.IsFailure)
            return false;

        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        ExperienceYears = experienceYears;
        Description = description;

        return true;
    }
    //------------------------------------Update Requisites---------------------------------------//
    public void UpdateRequisites(IEnumerable<RequisitesInfo> requisites)
    {
        Requisites = requisites.ToList();
    }
    //------------------------------------Update Social Networks----------------------------------//
    public void UpdateSocialNetworks(IEnumerable<SocialNetworkInfo> socialNetworkList)
    {
        SocialNetworks = new ValueObjectList<SocialNetworkInfo>(socialNetworkList);
    }
    public bool GetIsDeleted() => _isDeleted;
    public DateTime? GetDeletedTime() => _deletedDateTime;
}