using PetFamily.Domain.PetTypeManagment.Root;

namespace PetFamily.Application.Queries.PetType.GetListOfSpecies;

public record GetListOfSpeciesResponse(int SpeciesCount, List<Species> SpeciesList);
