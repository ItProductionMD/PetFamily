using PetFamily.Domain.DomainError;
using PetFamily.Domain.Shared.Validations;

namespace PetFamily.Domain.Results;
public class UnitResult : Result
{
    public static UnitResult Ok() => new() { IsSuccess = true };

    public static UnitResult ValidateCollection(params Func<UnitResult>[] validators)
    {

        var validationErrors = new List<ValidationError>();
        for (int i = 0; i < validators.Length; i++)
        {
            var result = validators[i]();

            if (result.IsFailure && result.Error != null && result.Error.ValidationErrors.Any())
                validationErrors.AddRange(result.Error.ValidationErrors);
        }
        return validationErrors.Count > 0
            ? Fail(Error.ValidationError(validationErrors))
            : Ok();
    }

    public Result<T> WithData<T>(T data)
    {
        Result<T> result = new Result<T>() { Data = data };
        if (IsFailure)
        {
            result.IsSuccess = false;
        }
        return result;
    }
}
