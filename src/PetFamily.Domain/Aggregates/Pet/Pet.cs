using CSharpFunctionalExtensions;
using PetFamily.Domain.Validation;
using PetFamily.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace PetFamily.Domain.Aggregates.Pet
{
    public class Pet : Entity<Guid>
    {
        public string Name { get; private set; }
        public PetType PetType { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public StatusForHelp StatusForHelp { get; private set; }
        public DateTime AnuntionDateTime { get; private set; }
        public string? Description { get; private set; }
        public ContactInfo ContactInfo { get; private set; }
        public PaymentDetails PaymentDetails { get; private set; }
        public HealthDetails HealthDetails { get; private set; }
        public AnimalTraits AnimalTraits { get; private set; }
        protected Pet()
        {

        }
        private Pet
            (string name,
            PetType type,
            DateTime dateOfBirth,
            ContactInfo contactInfo,
            PaymentDetails paymentDetails,
            AnimalTraits animalTraits,
            StatusForHelp? statusForHelp, 
            HealthDetails healthDetails,
            string? description) : base(Guid.NewGuid())
        {
            //Id = Guid.NewGuid();
            Name = name;
            PetType = type;    
            DateOfBirth = dateOfBirth;
            ContactInfo = contactInfo;
            AnimalTraits = animalTraits;
            StatusForHelp = statusForHelp??StatusForHelp.NoNeeds;
            HealthDetails = healthDetails;
            PaymentDetails = paymentDetails;
            Description = description;
            AnuntionDateTime = DateTime.UtcNow;
        }
        public void UpdateStatusForHelp(StatusForHelp newStatus)
        {
            StatusForHelp = newStatus;
        }
        public static Result<Pet> Create(CreatePetDto createPetDto)
        {
            var nameResult = ValidationExtensions.ValidateName(createPetDto.Name);
            if (nameResult.IsFailure)
                return Result.Failure<Pet>(nameResult.Error);
            if (createPetDto.DateOfBirth == null)
                return Result.Failure<Pet>("Date of birth cannot be null");
            var petTypeResult = PetType.Create(createPetDto.Species,createPetDto.Breed);
            if (petTypeResult.IsFailure)
                return Result.Failure<Pet>(petTypeResult.Error);
            if (!string.IsNullOrWhiteSpace(createPetDto.Description))
            {
                if (createPetDto.Description.Length>1000)
                    return Result.Failure<Pet>("Description cannot be more than 1000 characters");
            }

            var animalTraitsResult = AnimalTraits.Create(createPetDto.Weight ?? 0, createPetDto.Height ?? 0, createPetDto.Color);
            if (animalTraitsResult.IsFailure)
                return Result.Failure<Pet>(animalTraitsResult.Error);
            var contactInfoResult = ContactInfo.Create(createPetDto.Adress, createPetDto.OwnerPhone);
            if (contactInfoResult.IsFailure)
                return Result.Failure<Pet>(contactInfoResult.Error);

            var paymentDetailsResult = PaymentDetails.Create(createPetDto.PaymentName, createPetDto.PaymentDescription);
            if (paymentDetailsResult.IsFailure)
                return Result.Failure<Pet>(paymentDetailsResult.Error);

            var healthDetailsResult = HealthDetails.Create(createPetDto.IsVaccinated,createPetDto.IsNeutered,createPetDto.HealthStatus );
            if (healthDetailsResult.IsFailure)
                return Result.Failure<Pet>(healthDetailsResult.Error);
            var validateDescription = ValidationExtensions.ValidateDescription(createPetDto.Description);
            if (validateDescription.IsFailure)
                return Result.Failure<Pet>(validateDescription.Error);
            return Result.Success(new Pet
                (nameResult.Value,petTypeResult.Value,createPetDto.DateOfBirth.Value, 
                contactInfoResult.Value, paymentDetailsResult.Value, animalTraitsResult.Value, 
                createPetDto.StatusForHelp, healthDetailsResult.Value, 
                createPetDto.Description));

        }
    }

    public enum StatusForHelp
    {
        NeedsHelp,
        NeedsHome,
        NoNeeds
    }

}

