namespace PetSpecies.Public.Dtos;

public class SpeciesDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<BreedDto> BreedDtos { get; set; } = [];
}
