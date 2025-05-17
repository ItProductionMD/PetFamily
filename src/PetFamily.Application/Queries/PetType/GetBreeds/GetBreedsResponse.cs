using PetFamily.Domain.PetTypeManagment.Entities;

namespace PetFamily.Application.Queries.PetType.GetBreeds;

public record GetBreedsResponse(int BreedsCount, List<Breed> Breeds);

