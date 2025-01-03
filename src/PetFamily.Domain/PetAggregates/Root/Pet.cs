using PetFamily.Domain.DomainResult;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DTO;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;

namespace PetFamily.Domain.PetAggregates.Root;

public class Pet : Entity<Guid>
{
    public string Name { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public DateTime DateTimeCreated { get; private set; }
    public string? Description { get; private set; }
    public bool? IsVaccinated { get; private set; }
    public bool? IsSterilized { get; private set; }
    public double? Weight { get; private set; }
    public double? Height { get; private set; }
    public string? Color { get; private set; }
    public PetType PetType { get; private set; } //Species and Breed
    public PhoneNumber? OwnerPhone { get; private set; }
    public DonateDetails? DonateDetails { get; private set; }
    public string? HealthInfo { get; private set; }
    public Adress? Adress { get; private set; }
    public HelpStatus HelpStatus { get; private set; }
    public Volunteer Volunteer { get; private set; }//Navigation property
    private Pet(Guid id) : base(id) { }//Ef core needs this
    private Pet(Guid id, PetDomainDto petDomainDto) : base(id)
    {
        DateTimeCreated = DateTime.UtcNow;
        Name = petDomainDto.Name!;
        DateOfBirth = petDomainDto.DateOfBirth;
        Description = petDomainDto.Description;
        IsVaccinated = petDomainDto.IsVaccinated;
        IsSterilized = petDomainDto.IsSterilized;
        Weight = petDomainDto.Weight;
        Height = petDomainDto.Height;
        Color = petDomainDto.Color;
        PetType = petDomainDto.PetType!;
        OwnerPhone = petDomainDto.OwnerPhone;
        DonateDetails = petDomainDto.DonateDetails;
        HealthInfo = petDomainDto.HealthInfo;
        Adress = petDomainDto.Adress;
        HelpStatus = petDomainDto.HelpStatus!.Value;
    }
    public static Result<Pet> Create(PetDomainDto petDomainDto)
    {
        var validatePetDomain= Validate(petDomainDto);
        if (validatePetDomain.IsFailure)
            return Result<Pet>.Failure(validatePetDomain.Errors);
        return Result<Pet>.Success(new Pet(Guid.NewGuid(), petDomainDto));
    }
    public static Result Validate(PetDomainDto petDomainDto)
    {
        if (petDomainDto.Name == null)
            return Result.Failure("Name is required");
        if (petDomainDto.HelpStatus == null)
            return Result.Failure("HelpStatus is required");
        if (petDomainDto.PetType == null)
            return Result.Failure("PetType is required");
            //TODO: Add more validations
        return Result.Success();
    }
}
