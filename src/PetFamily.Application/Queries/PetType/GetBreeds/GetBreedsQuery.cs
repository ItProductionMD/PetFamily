using PetFamily.Application.Abstractions;
namespace PetFamily.Application.Queries.PetType.GetBreeds;

public class GetBreedsQuery : IQuery
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string SortBy { get; set; }
    public string SortDirection { get; set; }
    public Guid SpeciesId { get; set; }
    public GetBreedsQuery(int page, int pageSize, string? sortBy, string? sortDirection, Guid speciesId)
    {
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy?.ToLower() == "name" ? "name" : "id";
        SortDirection = sortDirection?.ToLower() == "asc" ? "asc" : "desc";
        SpeciesId = speciesId;
    }
}
