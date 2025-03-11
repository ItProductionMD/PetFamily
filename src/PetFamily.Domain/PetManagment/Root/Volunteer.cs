using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.Shared.Validations;
using PetFamily.Domain.Results;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.Enums;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using System.Collections.Generic;

namespace PetFamily.Domain.PetManagment.Root;

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
    private List<Pet> _pets { get; set; } = [];
    public bool IsDeleted => _isDeleted;
    private bool _isDeleted;
    public DateTime? DeletedDateTime => _deletedDateTime;

    private DateTime? _deletedDateTime;
    private Volunteer(Guid id) : base(id) { } //Ef core needs this

    public int PetsForAdoptCount => Pets.Where(p => p.HelpStatus == HelpStatus.ForAdoption).Count();
    public int PetsForHelpCount => Pets.Where(p => p.HelpStatus == HelpStatus.ForHelp).Count();
    public int PetsAdoptedCount => Pets.Where(p => p.HelpStatus == HelpStatus.Adopted).Count();

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
            () => ValidateRequiredObject(fullName, "Volunteer fullName"),
            () => ValidateRequiredObject(phone, "volunteer phone"),
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

        var affectedPets = Pets.Where(p =>
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
    public Pet CreateAndAddPet(
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
        Address address)
    {
        var serialNumberValue = Pets.Count + 1;
        var serialNumber = PetSerialNumber.Create(serialNumberValue, this).Data!;
        var pet = Pet.Create(
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
            helpStatus,
            healthInfo,
            address,
            serialNumber).Data!;

        _pets.Add(pet);
        return pet;
    }
    //--------------------------------------Remove Pet--------------------------------------------//
    public void RemovePet(Pet pet)
    {
        var maxSerialNumberValue = Pets.Max(pet => pet.SerialNumber.Value);

        var maxSerialNumber = PetSerialNumber.Create(maxSerialNumberValue, this);

        MovePetSerialNumber(pet, maxSerialNumber.Data!);

        _pets.Remove(pet);
    }
    //------------------------------Set is Deleted fal se(for soft deleting)-----------------------//
    public void Delete()
    {
        _isDeleted = true;
        _deletedDateTime = DateTime.UtcNow;

        foreach (var pet in Pets)
            pet.Delete();
    }
    //------------------------------Set is Deleted true(for soft deleting)-----------------------//
    public void Restore()
    {
        _isDeleted = false;
        _deletedDateTime = null;

        foreach (var pet in Pets)
            pet.Restore();
    }
    //------------------------------------Update Main Info----------------------------------------//
    public UnitResult UpdateMainInfo(
        FullName fullName,
        string email,
        Phone phoneNumber,
        int experienceYears,
        string description
        )
    {
        var validationResult = Validate(fullName, email, phoneNumber, experienceYears, description);
        if (validationResult.IsFailure)
            return validationResult;

        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        ExperienceYears = experienceYears;
        Description = description;

        return UnitResult.Ok();
    }
    //------------------------------------Update Requisites---------------------------------------//
    public void UpdateRequisites(IEnumerable<RequisitesInfo> requisites)
    {
        Requisites = requisites.ToList();
    }
    //------------------------------------Update Social Networks----------------------------------//
    public void UpdateSocialNetworks(IEnumerable<SocialNetworkInfo> socialNetworks)
    {
        SocialNetworks = socialNetworks.ToList();
    }
    public Pet GetPet(Guid petId)
    {
        var pet = Pets.FirstOrDefault(p => p.Id == petId);
        if (pet == null)
            throw new KeyNotFoundException($"Pet with id {petId} not found");
        return pet;
    }
    public void DeleteImagesFromPets(List<string> namesToDelete)
    {
        var imagesToDelete = namesToDelete.Select(n => Image.Create(n).Data!).ToList();
        foreach (var pet in Pets)
        {
            pet.DeleteImages(imagesToDelete);
        }
    }
}