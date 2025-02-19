﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Application.Species;

public record AddPetTypeRequest(string SpeciesName,IEnumerable<BreedDtos> BreedList);
public record BreedDtos(string Name,string Description);
public record UpdatePetTypeRequest(Guid speciesId,string SpeciesName,IEnumerable<string> BreedNames);