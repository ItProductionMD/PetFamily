using FluentValidation;
using FluentValidation.Results;
using PetFamily.Domain.Shared.DomainResult;

namespace PetFamily.Application.Validations;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result> validate)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            Result result = validate(value);

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
}
