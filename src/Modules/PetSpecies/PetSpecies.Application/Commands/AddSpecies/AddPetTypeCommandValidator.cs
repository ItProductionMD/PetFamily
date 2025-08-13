using PetFamily.SharedKernel.Results;
using PetSpecies.Application.Commands.AddSpecies;
using PetSpecies.Domain;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace PetFamily.SharedApplication.Commands.PetTypeManagement.AddPetType;

public static class AddPetTypeCommandValidator
{
    public static UnitResult Validate(AddPetTypeComand command)
    {
        return UnitResult.FromValidationResults(
            () => ValidateItems(command.BreedList, b => Breed.Validate(b.Name, b.Description)),
            () => Species.Validate(command.SpeciesName));
    }
}
