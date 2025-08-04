using PetFamily.SharedKernel.Errors;

namespace PetFamily.SharedApplication.Exceptions;

public sealed class ValidationException : Exception
{
    public Error Error { get;}

    public ValidationException(Error error) : base(error.Message)
    {
        Error = error;
    }
}
