using FluentValidation;
using FluentValidation.Results;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.Validations;

public static class ValidationExtensions
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, UnitResult> validate)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            UnitResult result = validate(value);

            if (result.IsSuccess)
                return;

            foreach (var error in result.Errors)
            {
                if (error != null)
                {
                    var failure = new ValidationFailure(error.FieldName, error.Message)
                    {
                        ErrorCode = error.Code
                    };
                    context.AddFailure(failure);
                }
            }
        });
    }
    public static Result<T> ToResultFailure<T>(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors;

        List<Error> myErrors = [];

        //convert fluentvalidation error in error from Result 
        foreach (var item in errors)
        {
            var customError = Error.Custom(
                item.ErrorCode,
                item.ErrorMessage,
                ErrorType.Validation,
                item.PropertyName);

            myErrors.Add(customError);
        }
        return Result.Fail(myErrors!);
    }

    public static UnitResult ToResultFailure(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors;

        List<Error> myErrors = [];

        //convert fluentvalidation error in error from Result 
        foreach (var item in errors)
        {
            var customError = Error.Custom(
                item.ErrorCode,
                item.ErrorMessage,
                ErrorType.Validation,
                item.PropertyName);

            myErrors.Add(customError);
        }
        return UnitResult.Fail(myErrors!);
    }
}
