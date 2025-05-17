using PetFamily.Domain.PetTypeManagment.Entities;
using PetFamily.Domain.Results;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using PetSpecies = PetFamily.Domain.PetTypeManagment.Root.Species;

namespace PetFamily.Application.Commands.PetTypeManagement.AddPetType;

public static class AddPetTypeCommandValidator
{
    public static UnitResult Validate(AddPetTypeComand command)
    {
        return UnitResult.ValidateCollection(
            () => ValidateItems(command.BreedList, b => Breed.Validate(b.Name, b.Description)),
            () => PetSpecies.Validate(command.SpeciesName));
    }
}
