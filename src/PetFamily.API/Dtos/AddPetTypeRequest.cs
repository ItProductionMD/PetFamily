using PetFamily.Application.Species;

namespace PetFamily.API.Dtos;

public record AddPetTypeRequest(string SpeciesName, IEnumerable<BreedDtos> BreedList)
{
    public AddPetTypeComand ToCommand() => new(SpeciesName, BreedList);
}
