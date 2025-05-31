using PetSpecies.Public.Dtos;
using Volunteers.Application.ResponseDtos;

namespace Volunteers.Application.Queries.GetPets;

public class GetPetsResponse
{
    public List<SpeciesDto>? SpeciesDtos { get; set; }
    public int TotalCount { get; set; }
    public List<PetWithVolunteerDto> Pets { get; set; } = new();
    public GetPetsResponse(int count, IEnumerable<PetWithVolunteerDto> pets)
    {
        TotalCount = count;
        Pets = pets.ToList();
    }
}
