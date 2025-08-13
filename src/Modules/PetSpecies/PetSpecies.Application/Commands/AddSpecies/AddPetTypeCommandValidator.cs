using PetFamily.SharedKernel.Results;
using PetSpecies.Application.Commands.AddSpecies;
using PetSpecies.Domain;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using PetFamily.SharedApplication.Exceptions;

namespace PetFamily.SharedApplication.Commands.PetTypeManagement.AddPetType;

public static class AddPetTypeCommandValidator
{
    public static void Validate(AddPetTypeComand command)
    {
        var validationResult =  UnitResult.FromValidationResults(
            () => ValidateItems(command.BreedList, b => Breed.Validate(b.Name, b.Description)),
            () => Species.Validate(command.SpeciesName));

        if(validationResult.IsFailure)
            throw new ValidationException(validationResult.Error);      
    }
}
