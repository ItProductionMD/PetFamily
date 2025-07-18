﻿using FluentValidation;
using FluentValidation.Results;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Validations;

namespace PetFamily.Application.Validations;

public static class ValidationExtensions
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, UnitResult> validate)
    {
        return ruleBuilder.Custom((Action<TElement, ValidationContext<T>>)((value, context) =>
        {
            UnitResult result = validate(value);

            if (result.IsSuccess)
                return;

            foreach (var validationError in result.Error.ValidationErrors)
            {
                if (validationError != null)
                {
                    var failure = new ValidationFailure((string)validationError.ObjectName, (string)validationError.ErrorCode)
                    {
                        ErrorCode = validationError.ErrorCode
                    };
                    context.AddFailure(failure);
                }
            }
        }));
    }

    public static Result<T> ToResultFailure<T>(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors;

        List<ValidationError> validationErrors = [];

        //convert fluentvalidation error in error from Result 
        foreach (var item in errors)
        {
            var validationError = new ValidationError(
                ValidationErrorType.General,
                item.PropertyName,
                item.ErrorCode);

            validationErrors.Add(validationError);
        }
        return Result.Fail(Error.FromValidationErrors(validationErrors));
    }

    public static UnitResult ToResultFailure(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors;

        List<ValidationError> validationErrors = [];

        //convert fluentvalidation error in error from Result 
        foreach (var item in errors)
        {
            var validationError = new ValidationError(
                ValidationErrorType.General,
                item.PropertyName,
                item.ErrorCode);

            validationErrors.Add(validationError);
        }
        return Result.Fail(Error.FromValidationErrors(validationErrors));
    }
}
