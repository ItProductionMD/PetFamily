using PetFamily.SharedApplication.Exceptions;
using Error = PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.SharedApplication.PaginationUtils;

public class PaginationParams
{
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
    public int Offset => (PageNumber - 1) * PageSize;
    public PaginationParams(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ValidationException(
                Error.ValueOutOfRange("Page number must be greater than or equal to 1"));

        if (pageSize < 1)
            throw new ValidationException(
                Error.ValueOutOfRange("Page size must be greater than or equal to 1"));

        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
