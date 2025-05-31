using PetSpecies.Application.Commands.AddSpecies;
using PetSpecies.Application.Commands.CommandsDtos;

namespace PetSpecies.Presentation.Requests;

public record AddPetTypeRequest(string SpeciesName, IEnumerable<BreedDtos> BreedList)
{
    public AddPetTypeComand ToCommand() => new(SpeciesName, BreedList);
}
