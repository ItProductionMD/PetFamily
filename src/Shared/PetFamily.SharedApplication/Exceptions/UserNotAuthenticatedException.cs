namespace PetFamily.SharedApplication.Exceptions;

public sealed class UserNotAuthenticatedException : Exception
{
    public UserNotAuthenticatedException() : base("User is not authenticated or user id is missing.")
    {

    }
}
