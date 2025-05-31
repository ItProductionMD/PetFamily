using PetSpecies.Public.Dtos;

namespace PetSpecies.Application.Queries.GetBreedPagedList;

public record BreedPagedListDto(int BreedsCount, List<BreedDto> Breeds);

