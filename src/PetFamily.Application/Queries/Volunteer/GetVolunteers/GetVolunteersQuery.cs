using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Queries.Volunteer.GetVolunteers;

public record GetVolunteersQuery(
    int pageSize,
    int pageNumber,
    string? orderBy,
    string? orderDirection) : IQuery;

public class GetVolunteersQueryV2
{
    public int PageSize { get; init; }
    public int Page { get; init; }
    public string OrderBy { get; init; }
    public string OrderDirection { get; init; }

    public GetVolunteersQueryV2(int pageSize, int page, string? orderBy, string orderDirection)
    {
        PageSize = pageSize;
        Page = page;
        OrderBy = orderBy switch
        {
            "full_name" => "full_name",
            "rating" => "rating",
            _ => "id"
        };
        OrderDirection = orderDirection.ToLower() == "asc" ? "asc" : "desc";
    }
}
