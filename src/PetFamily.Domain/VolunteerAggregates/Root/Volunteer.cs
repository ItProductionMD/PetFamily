using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.PetAggregates.Enums;
using PetFamily.Domain.PetAggregates.Root;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.ValueObjects;
using PetFamily.Domain.Shared.Validations;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;

namespace PetFamily.Domain.VolunteerAggregates.Root
{
    public class Volunteer : Entity<Guid>
    {
        public FullName FullName { get; private set; }
        public string Email { get; private set; }
        public Phone PhoneNumber { get; private set; }
        public int ExpirienceYears { get; private set; }
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
            int expirienceYears,
            string? description,
            ValueObjectList<DonateDetails>? donateDetailsList,
            ValueObjectList<SocialNetwork>? socialNetworksList
            ) : base(id)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            ExpirienceYears = expirienceYears;
            Description = description;
            DonateDetailsList = donateDetailsList ?? new(null);
            SocialNetworksList = socialNetworksList ?? new(null);
        }

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
            var validationResult = Validate(fullName, email, phoneNumber, expirienceYears, description);
            if (validationResult.IsFailure)
                return Result<Volunteer>.Failure(validationResult.Error!);

            var socialNetworks = socialNetworksList is null ? null : new ValueObjectList<SocialNetwork>(socialNetworksList);

            var donateDetails = donateDetailsList is null ? null : new ValueObjectList<DonateDetails>(donateDetailsList);

            return Result<Volunteer>
                .Success(new Volunteer(id.Value, fullName!, email!, phoneNumber!, expirienceYears, description, donateDetails, socialNetworks));
        }

        public static Result Validate(FullName? fullName, string? email, Phone? phoneNumber, int expirienceYears, string? description)
        {
            if (expirienceYears < 0)
                return Result.Failure(Error.CreateErrorInvalidFormat("Volunteer expirience years"));

            return ValidationExtensions.ValidateRequiredObject(fullName,"Volunteer fullName")

                .OnFailure(() => 
                    ValidationExtensions.ValidateRequiredObject(email,"Volunteer email"))

                .OnFailure(() =>
                    ValidateNonRequiredField(description,"Volunteer description", MAX_LENGTH_LONG_TEXT));
        }

        public void AddPet(Pet pet) => _pets.Add(pet);

        public int GetPetsCount() => _pets.Count;

        public int GetCountOfPetsForAdopt() => _pets.Where(p => p.HelpStatus == HelpStatus.ForAdopt).Count();

        public int GetCountOfPetsForHelp() => _pets.Where(p => p.HelpStatus == HelpStatus.ForHelp).Count();

        public int GetCountOfPetsAdopted() => _pets.Where(p => p.HelpStatus == HelpStatus.Adopted).Count();
    }
}