using PetFamily.Application.Dtos;
namespace PetFamily.Application.Queries.Pet.GetPets;

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
