using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Queries.PetType.GetListOfSpecies;

public class GetListOfSpeciesQuery: IQuery
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string SortBy { get; set; }
    public string SortDirection { get; set; }
    public GetListOfSpeciesQuery(int page, int pageSize, string sortBy, string sortDirection)
    {
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy.ToLower() == "name" ? "name" : "id";
        SortDirection = sortDirection.ToLower() == "asc" ? "asc" : "desc";
    }
}

