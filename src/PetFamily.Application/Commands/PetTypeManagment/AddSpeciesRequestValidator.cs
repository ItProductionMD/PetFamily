﻿using Microsoft.Extensions.Options;
using PetFamily.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetSpecies = PetFamily.Domain.PetManagment.Entities.Species;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using PetFamily.Domain.PetManagment.Entities;

namespace PetFamily.Application.Commands.PetTypeManagment;

public static class AddSpeciesRequestValidator
{
    public static UnitResult Validate(AddPetTypeComand request)
    {
        return UnitResult.ValidateCollection(
            () => ValidateItems(request.BreedList, b => Breed.Validate(b.Name, b.Description)),
            () => PetSpecies.Validate(request.SpeciesName));
    }
}
