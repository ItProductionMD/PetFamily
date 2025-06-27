namespace PetFamily.SharedKernel.Errors;

public enum ErrorType
{
    NotFound,
    Validation,
    Conflict,
    Exception,
    InternalServerError,
    Cancellation,
    Forbidden,
    Authentication,
    Authorization
}
