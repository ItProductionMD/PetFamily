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
using PetFamily.Domain.DomainError;
using System.Collections.Generic;
using System.Numerics;
using PetFamily.Domain.Shared.Interfaces;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace PetFamily.Domain.PetManagment.Root;

public class Volunteer : Entity<Guid>, ISoftDeletable, IHasUniqueFields
{
    public FullName FullName { get; private set; }
    [Unique]
    public string Email { get; private set; }
    [Unique]
    public Phone Phone { get; private set; }
    public int ExperienceYears { get; private set; }
    public string? Description { get; private set; }
    public int Rating { get; private set; }
    public IReadOnlyList<RequisitesInfo> Requisites { get; private set; }
    public IReadOnlyList<SocialNetworkInfo> SocialNetworks { get; private set; }
    public IReadOnlyList<Pet> Pets => _pets;
    private List<Pet> _pets { get; set; } = [];
    public bool IsDeleted => _isDeleted;
    private bool _isDeleted;
    public DateTime? DeletedDateTime => _deletedDateTime;

    private DateTime? _deletedDateTime;
    private Volunteer(Guid id) : base(id) { } //Ef core needs this

    public int PetsForAdoptCount => Pets
        .Where(p => p.HelpStatus == HelpStatus.ForAdoption && p.IsDeleted == false)
        .Count();
    public int PetsForHelpCount => Pets
        .Where(p => p.HelpStatus == HelpStatus.ForHelp && p.IsDeleted == false)
        .Count();
    public int PetsAdoptedCount => Pets
        .Where(p => p.HelpStatus == HelpStatus.Helped && p.IsDeleted == false)
        .Count();

    public int ExistingPetsCount => Pets
        .Where(p => p.IsDeleted == false)
        .Count();

    public static List<string> UniquenessFieldsName { get => ["Email", "PhoneNumber"]; }

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
        Phone = phoneNumber;
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
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new Volunteer(
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

        var affectedPets = Pets.Where(p => p.IsDeleted == false &&
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
        var serialNumberValue = ExistingPetsCount + 1;
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

    //--------------------------------------Soft delete Pet---------------------------------------//
    public void SoftDeletePet(Pet pet)
    {
        var maxSerialNumberValue = ExistingPetsCount;

        var maxSerialNumber = PetSerialNumber.Create(maxSerialNumberValue, this).Data!;

        MovePetSerialNumber(pet, maxSerialNumber);

        pet.SoftDelete();
    }

    //-------------------------------------Hard delete pet----------------------------------------//
    public void HardDeletePet(Pet pet)
    {
        var maxSerialNumberValue = Pets.Max(pet => pet.SerialNumber.Value);

        var maxSerialNumber = PetSerialNumber.Create(maxSerialNumberValue, this);

        MovePetSerialNumber(pet, maxSerialNumber.Data!);

        _pets.Remove(pet);
    }

    //----------------------------------------Soft delete pet-------------------------------------//
    public void SoftDelete()
    {
        _isDeleted = true;
        _deletedDateTime = DateTime.UtcNow;

        foreach (var pet in _pets)
            pet.SoftDelete();
    }


    //-------------------------------------------Restore------------------------------------------//
    public void Restore()
    {
        _isDeleted = false;
        _deletedDateTime = null;

        foreach (var pet in _pets)
            pet.Restore();
    }
    //-----------------------------------------Restore Pet----------------------------------------//
    public UnitResult RestorePet(Guid petId)
    {
        var pet = _pets.FirstOrDefault(p => p.Id == petId);
        if (pet == null)
            return UnitResult.Fail(Error.NotFound($"Pet with id:{petId}"));

        var newSerialNumberValue = ExistingPetsCount + 1;
        
        pet.Restore();

        PetSerialNumber serial = PetSerialNumber.Create(newSerialNumberValue, this).Data!;

        pet.SetSerialNumber(serial);

        return UnitResult.Ok();
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
        Phone = phoneNumber;
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
        foreach (var pet in _pets)
        {
            pet.DeleteImages(imagesToDelete);
        }
    }

    //------------------------------------ChangePetsOrder-----------------------------------------//
    public UnitResult ChangePetsOrder(List<Guid> petsIds)
    {
        if (_pets.Count != petsIds.Count)
            return UnitResult.Fail(Error.ValueOutOfRange("Count of pets in PetsOrder"));

        foreach (var id in petsIds)
        {
            var pet = _pets.FirstOrDefault(p => p.Id == id);
            if (pet == null)
                return UnitResult.Fail(Error.NotFound($"Pet with id {id}"));

            var newSerialNumber = PetSerialNumber.Create(petsIds.IndexOf(id) + 1, this).Data;
            if (newSerialNumber == null)
                return UnitResult.Fail(Error.InvalidFormat("pet serial number"));

            pet.SetSerialNumber(newSerialNumber);
        }
        return UnitResult.Ok();
    }

    public UnitResult UpdatePetStatus(Guid petId, HelpStatus helpStatus)
    {
        var pet = Pets.FirstOrDefault(p => p.Id == petId);
        if (pet == null)
            return UnitResult.Fail(Error.NotFound("Pet with id:{petId}"));

        pet.ChangePetStatus(helpStatus);
        if (helpStatus == HelpStatus.Helped)
            UpdateRating();

        return UnitResult.Ok();
    }

    private void UpdateRating()
    {
        Rating = PetsAdoptedCount;
    }
    public static List<string> VolunteerUniqness() => Volunteer.UniquenessFieldsName;

    public static string[] GetUniqueFields()
    {
        return typeof(Volunteer).GetProperties()
        .Where(prop => Attribute.IsDefined(prop, typeof(UniqueAttribute)))
        .Select(prop => prop.Name.ToLower())
        .ToArray();
    }

    public static UniqueProps[] GetUniqueProps()
    {
        var props = typeof(Volunteer).GetProperties();
        List<UniqueProps> uniqueProps = [];
        foreach (var prop in props)
        {
            if (Attribute.IsDefined(prop, typeof(UniqueAttribute)))
            {
                UniqueProps uniqueProp = new() { Field = prop.Name };
                if (typeof(IValueObject).IsAssignableFrom(prop.PropertyType))
                {
                    var vProps = prop.PropertyType.GetProperties();
                    foreach (var valueObjectProp in vProps)
                    {
                        uniqueProp.Values.Add(valueObjectProp.Name.ToLower());
                    }
                }
                else
                {
                    uniqueProp.Values.Add(prop.Name);
                }
                uniqueProps.Add(uniqueProp);
            }
        }
        return uniqueProps.ToArray();
    }

}
