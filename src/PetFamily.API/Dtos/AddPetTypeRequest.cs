using PetFamily.Application.Commands.PetTypeManagement;
using PetFamily.Application.Commands.PetTypeManagement.AddPetType;

namespace PetFamily.API.Dtos;

public record AddPetTypeRequest(string SpeciesName, IEnumerable<BreedDtos> BreedList)
{
    public AddPetTypeComand ToCommand() => new(SpeciesName, BreedList);
}
