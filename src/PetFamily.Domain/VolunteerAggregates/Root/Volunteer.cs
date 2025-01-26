using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.PetAggregates.Enums;
using PetFamily.Domain.PetAggregates.Root;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.ValueObjects;
using PetFamily.Domain.Shared.Validations;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;

namespace PetFamily.Domain.VolunteerAggregates.Root;

public class Volunteer : Entity<Guid>, ISoftDeletable
{
    public FullName FullName { get; private set; }
    public string Email { get; private set; }
    public Phone PhoneNumber { get; private set; }
    public int ExperienceYears { get; private set; }
    public string? Description { get; private set; }
    public ValueObjectList<DonateDetails> DonateDetailsList { get; private set; }
    public ValueObjectList<SocialNetwork> SocialNetworksList { get; private set; }
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
        ValueObjectList<DonateDetails>? donateDetailsList,
        ValueObjectList<SocialNetwork>? socialNetworksList
        ) : base(id)
    {
        FullName = fullName;
        Email = email.Trim();
        PhoneNumber = phoneNumber;
        ExperienceYears = experienceYears;
        Description = description?.Trim();
        DonateDetailsList = donateDetailsList ?? new(null); // List Can be empty
        SocialNetworksList = socialNetworksList ?? new(null);//list can be emty
    }

    //--------------------------------------Factory method----------------------------------------//
    public static Result<Volunteer> Create(
        VolunteerID id,
        FullName? fullName,
        string? email,
        Phone? phoneNumber,
        int expirienceYears,
        string? description,
        IReadOnlyList<DonateDetails>? donateDetailsList,
        IReadOnlyList<SocialNetwork>? socialNetworksList
        )
    {
        //---------------------------------Validate Volunteer-------------------------------------//

        var validationResult = Validate(fullName, email, phoneNumber, expirienceYears, description);

        if (validationResult.IsFailure)
            return Result<Volunteer>.Failure(validationResult.Errors!);

        //-------------Create ValueObjectList<valueObject> from IReadOnlyList<valueObject>--------//

        var socialNetworks = new ValueObjectList<SocialNetwork>(socialNetworksList);

        var donateDetails = new ValueObjectList<DonateDetails>(donateDetailsList);

        return Result<Volunteer>
            .Success(new Volunteer(
                id.Value, fullName!,
                email!, phoneNumber!,
                expirienceYears,
                description,
                donateDetails,
                socialNetworks));
    }
    //-------------------------------------Validation method--------------------------------------//
    public static Result Validate(
        FullName? fullName,
        string? email,
        Phone? phone,
        int experienceYears,
        string? description)
    {
        return Result.ValidateCollection(

            () => ValidateRequiredObject(fullName, "Volunteer fullName"),
            () => ValidateRequiredObject(phone, "volunteer phone"),
            () => ValidateNumber(experienceYears, "volunteer experienece", 0, 100),
            () => ValidateRequiredField(email, "Volunteer email", MAX_LENGTH_SHORT_TEXT, EMAIL_PATTERN),
            () => ValidateNonRequiredField(description, "Volunteer description", MAX_LENGTH_LONG_TEXT));
    }
    //--------------------------------------Add Pet-----------------------------------------------//
    public void AddPet(Pet pet) => _pets.Add(pet);
    //--------------------------------------Remove Pet--------------------------------------------//
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
    //------------------------------Set is Deleted flase(for soft deleting)-----------------------//
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

    //------------------------------------Update Donate Details-----------------------------------//
    public void UpdateDonateDetails(IEnumerable<DonateDetails>? donateDetailsList)
    {   
        DonateDetailsList = new ValueObjectList<DonateDetails>(donateDetailsList);
    }
    //------------------------------------Update Social Networks----------------------------------//
    public void UpdateSocialNetworks(IEnumerable<SocialNetwork> socialNetworkList)
    {
        SocialNetworksList = new ValueObjectList<SocialNetwork>(socialNetworkList);
    }
}