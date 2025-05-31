using PetSpecies.Public.Dtos;

namespace Volunteers.Application.ResponseDtos;

public record PetPagedList(List<SpeciesDto>? SpeciesDtos = null);
