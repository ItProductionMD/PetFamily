using PetFamily.Domain.DomainResult;
using PetFamily.Domain.PetAggregates.Root;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Domain.VolunteerAggregates.Root
{
    public class Volunteer : Entity<Guid>
    {
        public FullName FullName { get; private set; }
        public string Email { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public int ExpirienceYears { get; private set; }
        public string? Description { get; private set; }
        public DonateDetails? DonateDetails { get; private set; }
        public IReadOnlyList<Pet> Pets=>_pets;
        private readonly List<Pet> _pets = [];
       
        private Volunteer(Guid id) : base(id) { } //Ef core needs this

        private Volunteer(Guid id, FullName fullName, string email, PhoneNumber phoneNumber, int expirienceYears, string? description, DonateDetails? donateDetails) : base(id)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            ExpirienceYears = expirienceYears;
            Description = description;
            DonateDetails = donateDetails;
        }
        public static Result<Volunteer> Create(Guid id,FullName? fullName, string? email,PhoneNumber? phoneNumber,int expirienceYears,string? description,DonateDetails? donateDetails)
        {
            var validationResult = Validate(fullName, email, phoneNumber, expirienceYears, description, donateDetails);
            if (validationResult.IsFailure)
                return Result<Volunteer>.Failure(validationResult.Errors);
            return Result<Volunteer>.Success(new Volunteer(id, fullName!, email!, phoneNumber!, expirienceYears, description, donateDetails));

        }
        public static Result Validate(FullName? fullName, string? email, PhoneNumber? phoneNumber, int expirienceYears, string? description, DonateDetails? donateDetails)
        {
            if (fullName == null)
                return Result.Failure("FullName is required");
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure("Email is required");
            if (phoneNumber == null)
                return Result.Failure("PhoneNumber is required");
            if (expirienceYears < 0)
                return Result.Failure("ExpirienceYears must be greater than 0");
            //TODO: Add more validations
            return Result.Success();
        }
        public void AddPet(Pet pet) => _pets.Add(pet);
        public int GetPetsCount() => _pets.Count;
        public int GetCountOfPetsForAdopt() => _pets.Where(p => p.HelpStatus == HelpStatus.ForAdopt).Count();
        public int GetCountOfPetsForHelp() => _pets.Where(p => p.HelpStatus == HelpStatus.ForHelp).Count();
        public int GetCountOfPetsAdopted() => _pets.Where(p => p.HelpStatus == HelpStatus.Adopted).Count();
    }
}
