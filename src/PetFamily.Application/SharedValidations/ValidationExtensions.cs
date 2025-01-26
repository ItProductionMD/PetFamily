using FluentValidation.Results;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Application.Volunteers.SharedVolunteerRequests;

namespace PetFamily.Application.Validations;

public static class ValidationExtensions
{
    public static Result<T> ToResultFailure<T>(this ValidationResult validationResult)
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

    public static Result ToResultFailure(this ValidationResult validationResult)
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
        return Result.Failure(myErrors!);
    }

    public static Result ValidateDonateDetails(DonateDetailsRequest request) =>
           DonateDetails.Validate(request.Name, request.Description);

    public static Result ValidateSocialNetwork(SocialNetworksRequest request) =>
        SocialNetwork.Validate(request.Name, request.Url);
}
