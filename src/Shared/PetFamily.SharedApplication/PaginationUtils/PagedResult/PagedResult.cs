namespace PetFamily.SharedApplication.PaginationUtils.PagedResult;

public class PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public PagedResult(IReadOnlyCollection<T> items, int totalCount, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    }
}

