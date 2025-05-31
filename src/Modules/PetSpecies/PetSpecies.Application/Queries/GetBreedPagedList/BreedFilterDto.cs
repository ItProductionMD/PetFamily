using PetFamily.Application.Dtos;

namespace PetSpecies.Application.Queries.GetBreedPagedList;

public class BreedFilterDto
{
    public string? SearchName { get; set; }
    public List<OrderBy<BreedProperty>>? OrderByes { get; set; }
}

public enum BreedProperty
{
    Name,
    Id
}
