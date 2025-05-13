namespace PetFamily.Application.Dtos;

public class SpeciesDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public List<BreedDto> BreedDtos { get; set; } = new();
}
