namespace PetFamily.SharedApplication.PaginationUtils.PagedResult;

public enum PagingMode
{
    SingleQuery, // COUNT(*) AND OVER
    TwoQueries   // Separately COUNT
}
