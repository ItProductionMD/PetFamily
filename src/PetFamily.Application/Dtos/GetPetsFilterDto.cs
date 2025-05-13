using static PetFamily.Domain.Shared.Validations.ValidationConstants;
namespace PetFamily.Application.Dtos;

public record GetPetsFilterDto(List<SpeciesDto>? SpeciesDtos = null);
