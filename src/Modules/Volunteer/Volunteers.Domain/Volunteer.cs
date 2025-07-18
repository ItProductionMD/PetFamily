﻿using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Uniqness;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using Volunteers.Domain.Enums;
using Volunteers.Domain.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace Volunteers.Domain;

public class Volunteer : SoftDeletable, IEntity<Guid>, IHasUniqueFields
{
    public Guid Id { get;private set; }
    public FullName FullName { get; private set; }
    [Unique]
    public string Phone { get; private set; }
    public int ExperienceYears { get; private set; }
    public string? Description { get; private set; }
    public int Rating { get; private set; }
    public IReadOnlyList<RequisitesInfo> Requisites { get; private set; }
    public IReadOnlyList<Pet> Pets => _pets;
    private List<Pet> _pets { get; set; } = [];
    public UserId UserId { get; private set; }
    private Volunteer() { } //Ef core needs this
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

    private Volunteer(
        Guid id,
        UserId userId,
        FullName fullName,
        int experienceYears,
        string? description,
        string phone,
        List<RequisitesInfo> requisites)
    {
        Id = id;
        UserId = userId;
        FullName = fullName;
        Phone = phone;
        ExperienceYears = experienceYears;
        Description = description?.Trim();
        Requisites = requisites; // List Can be empty
    }
    //--------------------------------------Factory method----------------------------------------//
    public static Result<Volunteer> Create(
        VolunteerID id,
        UserId userId,
        FullName fullName,
        int expirienceYears,
        string? description,
        Phone phone,
        List<RequisitesInfo> requisites
        )
    {
        var validationResult = Validate(fullName, expirienceYears, description);
        if (validationResult.IsFailure)
            return validationResult;

        var validatePhone = ValidateRequiredObject(phone, "Volunteer phone");
        if (validatePhone.IsFailure)
            return validatePhone;

        var stringPhone = phone.ToString();

        return Result.Ok(new Volunteer(
                id.Value,
                userId,
                fullName!,
                expirienceYears,
                description,
                stringPhone,
                requisites));
    }

    public static UnitResult Validate(
        FullName? fullName,
        int experienceYears,
        string? description)
    {
        return UnitResult.FromValidationResults(
            () => ValidateRequiredObject(fullName, "Volunteer fullName"),
            () => ValidateIntegerNumber(experienceYears, "volunteer experienece", 0, 100),
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
        string? healthInfo)
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
            Address.CreateEmpty(),
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

        pet.SetSerialNumber(PetSerialNumber.Empty());

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

    //----------------------------------------Soft delete itself----------------------------------//
    override public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;

        foreach (var pet in _pets)
            pet.SoftDelete();
    }


    //--------------------------------------Restore itself----------------------------------------//
    override public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;

        foreach (var pet in _pets)
        {
            if (pet.SerialNumber != PetSerialNumber.Empty())
                pet.Restore();
        }
    }

    //-----------------------------------------Restore Pet----------------------------------------//
    public UnitResult RestorePet(Guid petId)
    {
        var pet = _pets.FirstOrDefault(p => p.Id == petId && p.IsDeleted == true);
        if (pet == null)
            return UnitResult.Fail(Error.NotFound($"Pet with id:{petId}"));

        var newSerialNumberValue = ExistingPetsCount + 1;

        pet.Restore();

        PetSerialNumber serial = PetSerialNumber.Create(newSerialNumberValue, this).Data!;

        pet.SetSerialNumber(serial);

        return UnitResult.Ok();
    }

    //---------------------------------------Restore Deleted Pets---------------------------------//
    public UnitResult RestoreDeletedPets()
    {
        var deletedPets = _pets.Where(p => p.IsDeleted).ToList();
        if (deletedPets.Count == 0)
            return UnitResult.Fail(Error.NotFound("No deleted pets found"));

        foreach (var pet in deletedPets)
        {
            pet.Restore();
            var newSerialNumberValue = ExistingPetsCount + 1;
            PetSerialNumber serial = PetSerialNumber.Create(newSerialNumberValue, this).Data!;
            pet.SetSerialNumber(serial);
        }
        return UnitResult.Ok();
    }

    //------------------------------------Update Main Info----------------------------------------//
    public UnitResult UpdateMainInfo(
        FullName fullName,
        int experienceYears,
        string description
        )
    {
        var validationResult = Validate(fullName, experienceYears, description);
        if (validationResult.IsFailure)
            return validationResult;

        FullName = fullName;
        ExperienceYears = experienceYears;
        Description = description;

        return UnitResult.Ok();
    }

    //------------------------------------Update Requisites---------------------------------------//
    public void UpdateRequisites(IEnumerable<RequisitesInfo> requisites)
    {
        Requisites = requisites.ToList();
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

    public Result<UnitResult> UpdatePhone(Phone newPhone)
    {
        var validateRequiredPhone = ValidateRequiredObject(newPhone,"Phone for volunteer");
        if (validateRequiredPhone.IsFailure)
            return Result.Fail(validateRequiredPhone.Error);

        Phone = newPhone.ToString();

        return UnitResult.Ok();
    }

    public UnitResult UpdatePet(Guid petId, UpdatePetInfo updatePetInfo)
    {
        var pet = GetPet(petId);
        if (pet == null)
            return UnitResult.Fail(Error.NotFound($"Pet with id:{petId} in volunteer(id: {Id})"));

        var updateResult = pet.Update(updatePetInfo);
        return updateResult;
    }

}
