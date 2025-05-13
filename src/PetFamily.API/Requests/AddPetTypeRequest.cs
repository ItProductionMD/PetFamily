using PetFamily.Application.Commands.PetTypeManagement;
using PetFamily.Application.Commands.PetTypeManagement.AddPetType;

namespace PetFamily.API.Requests;

public record AddPetTypeRequest(string SpeciesName, IEnumerable<BreedDtos> BreedList)
{
    public AddPetTypeComand ToCommand() => new(SpeciesName, BreedList);
}
