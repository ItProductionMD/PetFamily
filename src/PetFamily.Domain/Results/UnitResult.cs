using PetFamily.Domain.DomainError;
using System.Runtime.CompilerServices;

namespace PetFamily.Domain.Results;
public class UnitResult : Result
{
    public UnitResult WithError(Error error)
    {
        AddError(error);
        return this;
    }
    public static UnitResult Ok() => new() { IsSuccess = true };
    public static UnitResult ValidateCollection(params Func<UnitResult>[] validators)
    {
        var errors = new List<Error>();
        for (int i = 0; i < validators.Length; i++)
        {
            var result = validators[i]();

            if (result.IsFailure && result.Errors != null)
                errors.AddRange(result.Errors);
        }
        return errors.Count > 0 
            ? Fail(errors) 
            : Ok();
    }
    public  Result<T> WithData<T>(T data)
    {
        Result<T> result = new Result<T>() { Data = data };
        if (IsFailure)
        {
            result.IsSuccess = false;
            result.AddErrors(Errors);
        }
        return result;
    }
}
