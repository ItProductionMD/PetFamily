﻿using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.PetAggregates.Enums;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DTO;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using PetFamily.Domain.Shared.Validations;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;

namespace PetFamily.Domain.PetAggregates.Root;
public class Pet : Entity<Guid>
{
    public string Name { get; private set; }             
    public DateOnly? DateOfBirth { get; private set; }
    public DateTime DateTimeCreated { get; private set; }
    public string? Description { get; private set; }
    public bool IsVaccinated { get; private set; }
    public bool IsSterilized { get; private set; }
    public double Weight { get; private set; }
    public double Height { get; private set; }
    public string? Color { get; private set; }
    public PetType PetType { get; private set; } //Species and Breed
    public Phone? OwnerPhone { get; private set; }
    public DonateDetails? DonateDetails { get; private set; }
    public string? HealthInfo { get; private set; }
    public Adress? Adress { get; private set; }
    public HelpStatus HelpStatus { get; private set; }
    public Volunteer Volunteer { get; private set; }//Navigation property
    private const string NAME = "Pet nickname";
    private const string DESCRIPTION = "Pet description";
    private const string TYPE = "Pet type";
    private const string HEALTH_INFO="Pet healthInfo";
    private const string COLOR = "Pet color";
    private Pet(Guid id) : base(id) { }//Ef core needs this
    private Pet(Guid id, PetDomainDto petDomainDto) : base(id)
    {
        DateTimeCreated = DateTime.UtcNow;
        Name = petDomainDto.Name!;
        DateOfBirth = petDomainDto.DateOfBirth;
        Description = petDomainDto.Description;
        IsVaccinated = GetBoolValue(petDomainDto.IsVaccinated);
        IsSterilized = GetBoolValue(petDomainDto.IsSterilized);
        Weight = petDomainDto.Weight;
        Height = petDomainDto.Height;
        Color = petDomainDto.Color;
        PetType = petDomainDto.PetType!;
        OwnerPhone = petDomainDto.OwnerPhone;
        DonateDetails = petDomainDto.DonateDetails;
        HealthInfo = petDomainDto.HealthInfo;
        Adress = petDomainDto.Adress;
        HelpStatus = petDomainDto.HelpStatus;
    }
    public static Result<Pet> Create(PetDomainDto petDomainDto)
    {
        var validatePetDomain= Validate(petDomainDto);
        if (validatePetDomain.IsFailure)
            return Result<Pet>.Failure(validatePetDomain.Error!);
        return Result<Pet>.Success(new Pet(Guid.NewGuid(), petDomainDto));
    }
    public static Result Validate(PetDomainDto petDomainDto)=>
        ValidateRequiredField(petDomainDto.Name, NAME, MAX_LENGTH_SHORT_TEXT, NAME_PATTERN)
        .OnFailure(()=>ValidationExtensions.ValidateRequiredObject(petDomainDto.PetType, TYPE))
        .OnFailure(()=>ValidateNonRequiredField(petDomainDto.Description, DESCRIPTION, MAX_LENGTH_LONG_TEXT))
        .OnFailure(()=>ValidateNonRequiredField(petDomainDto.HealthInfo, HEALTH_INFO, MAX_LENGTH_LONG_TEXT))
        .OnFailure(()=>ValidateNonRequiredField(petDomainDto.Color, COLOR, MAX_LENGTH_SHORT_TEXT));
    //TODO: Add more validations
    private static bool GetBoolValue(bool? variable)
    {
        if (variable.HasValue)
            return variable.Value;
        else return false;
    }
}
