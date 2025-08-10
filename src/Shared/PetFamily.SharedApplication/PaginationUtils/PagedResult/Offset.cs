namespace PetFamily.SharedApplication.PaginationUtils.PagedResult;

public static class Offset
{
    public static int Calculate(int page, int pageSize) => (page - 1) * pageSize;
}

