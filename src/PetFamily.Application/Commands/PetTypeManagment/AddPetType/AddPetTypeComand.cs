using PetFamily.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Application.Commands.PetTypeManagment.AddPetType;

public record AddPetTypeComand(string SpeciesName, IEnumerable<BreedDtos> BreedList) : ICommand;
