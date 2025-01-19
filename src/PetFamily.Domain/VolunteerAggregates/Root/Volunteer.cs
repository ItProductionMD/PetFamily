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

public class Volunteer : Entity<Guid>
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

    //------------------------------------------Factory method----------------------------------------------//
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
        //------------------------------------Validation---------------------------------------------//

        var validationResult = Validate(fullName, email, phoneNumber, expirienceYears, description);

        if (validationResult.IsFailure)
            return Result<Volunteer>.Failure(validationResult.Errors!);

        //-----------------Create ValueObjectList<valueObject> from IReadOnlyList<valueObject>--------//

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

    public static Result Validate(FullName? fullName, string? email, Phone? phone, int experienceYears, string? description)
    {
        return Result.ValidateCollection(

            () => ValidateNumber(experienceYears, "volunteer experienece", 0, 100),

            () => ValidationExtensions.ValidateRequiredObject(fullName, "Volunteer fullName"),

            () => ValidationExtensions.ValidateRequiredObject(phone, "volunteer phone"),

            () => ValidateRequiredField(email, "Volunteer email", MAX_LENGTH_SHORT_TEXT, EMAIL_PATTERN),

            () => ValidateNonRequiredField(description, "Volunteer description", MAX_LENGTH_LONG_TEXT, pattern: null));
    }

    public void AddPet(Pet pet) => _pets.Add(pet);

    public int GetPetsCount() => _pets.Count;

    public int GetCountOfPetsForAdopt() => _pets.Where(p => p.HelpStatus == HelpStatus.ForAdopt).Count();

    public int GetCountOfPetsForHelp() => _pets.Where(p => p.HelpStatus == HelpStatus.ForHelp).Count();

    public int GetCountOfPetsAdopted() => _pets.Where(p => p.HelpStatus == HelpStatus.Adopted).Count();
}