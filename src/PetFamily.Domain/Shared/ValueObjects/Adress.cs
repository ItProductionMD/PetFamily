using PetFamily.Domain.DomainResult;
using PetFamily.Domain.Shared.DTO;
using PetFamily.Domain.Shared.Validations;

namespace PetFamily.Domain.Shared.ValueObjects
{
    public record Adress
    {
        public string Street { get; }
        public string City { get; }
        public string Country { get; }
        public string Number { get; }
        private Adress() { }//EF core need this
        private Adress(AdressDomainDTO adressDto)
        {
            Country = adressDto.Country!;
            City = adressDto.City!;
            Street = adressDto.Street!;
            Number = adressDto.Number!;
        }
        public static Result<Adress> Create(AdressDomainDTO adressDto)
        {
            var validationResult = Validate(adressDto);
            if (validationResult.IsFailure)
                return Result<Adress>.Failure(validationResult.Errors);
            return Result<Adress>.Success(new Adress(adressDto));
        }
        private static Result Validate(AdressDomainDTO adress)
        {
            var nullOrEmptyValidationResult = ValidationExtensions.ValidateIfStringsNotEmpty(adress.DictionaryForValidate);
            if (nullOrEmptyValidationResult.IsFailure)
                return nullOrEmptyValidationResult;
            //ToDo: Add more validations    
            return Result.Success();
        }
    }

}
