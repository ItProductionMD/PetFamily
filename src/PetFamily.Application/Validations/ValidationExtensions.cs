using FluentValidation.Results;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;

namespace PetFamily.Application.Validations;

public static class ValidationExtensions
{
    public static Result<T> Failure<T>(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors;

        List<Error> myErrors = [];

        //convert fluentvalidation error in error from Result 
        foreach (var item in errors)
        {
            var customError = Error.CreateCustomError(
                item.ErrorCode,
                item.ErrorMessage,
                ErrorType.Validation,
                item.PropertyName);

            myErrors.Add(customError);
        }
        return Result<T>.Failure(myErrors!);
    }
}
