using PetFamily.SharedApplication.Abstractions.CQRS;
using PetSpecies.Application.Commands.CommandsDtos;

namespace PetSpecies.Application.Commands.AddSpecies;

public record AddPetTypeComand(string SpeciesName, IEnumerable<NewBreedDto> BreedList) : ICommand;
