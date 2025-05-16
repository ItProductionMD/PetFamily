namespace PetFamily.SharedKernel.Domain.DomainError;

public enum ErrorType
{
    NotFound,
    Validation,
    Conflict,
    Exception,
    InternalServerError,
    Cancellation
}
