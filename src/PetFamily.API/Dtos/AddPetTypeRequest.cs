using PetFamily.Application.Commands.PetTypeManagment;
using PetFamily.Application.Commands.PetTypeManagment.AddPetType;

namespace PetFamily.API.Dtos;

public record AddPetTypeRequest(string SpeciesName, IEnumerable<BreedDtos> BreedList)
{
    public AddPetTypeComand ToCommand() => new(SpeciesName, BreedList);
}
