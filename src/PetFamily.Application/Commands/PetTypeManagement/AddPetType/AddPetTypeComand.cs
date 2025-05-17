using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetTypeManagement.AddPetType;

public record AddPetTypeComand(string SpeciesName, IEnumerable<BreedDtos> BreedList) : ICommand;
