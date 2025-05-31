using PetSpecies.Public.Dtos;

namespace PetSpecies.Application.Queries.GetSpeciesPagedList;

public record SpeciesPagedListDto(int SpeciesCount, List<SpeciesDto> SpeciesList);




