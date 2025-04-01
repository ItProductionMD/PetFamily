using Microsoft.Extensions.Options;
using PetFamily.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetSpecies = PetFamily.Domain.PetTypeManagment.Root.Species;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using PetFamily.Domain.PetTypeManagment.Entities;

namespace PetFamily.Application.Commands.PetTypeManagment.AddPetType;

public static class AddPetTypeCommandValidator
{
    public static UnitResult Validate(AddPetTypeComand command)
    {
        return UnitResult.ValidateCollection(
            () => ValidateItems(command.BreedList, b => Breed.Validate(b.Name, b.Description)),
            () => PetSpecies.Validate(command.SpeciesName));
    }
}
