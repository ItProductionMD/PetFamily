using PetFamily.SharedKernel.Errors;
using System.Text;

namespace PetFamily.SharedKernel.Results;
public abstract class Result
{
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;

    public Error Error { get; set; }

    protected Result() { }

    public static Result<T> Ok<T>(T? data = default) => new() { IsSuccess = true, Data = data };

    public static Result<T> Ok<T>() => new() { IsSuccess = true };

    public static UnitResult Fail(Error error) => new() { IsSuccess = false, Error = error };

    public static UnitResult Fail(List<Error> errors)
    {
        var result = new UnitResult();
        //result.Error = Error.FileValidation();
        result.AddValidationErrors(errors!);
        return result;
    }

    public void SetToFailure()
    {
        IsSuccess = false;
    }

    public void AddValidationError(Error error)
    {
        Error.ValidationErrors.AddRange(error.ValidationErrors);

        if (Error.ValidationErrors.Count > 0)
            SetToFailure();
    }

    public string ValidationMessagesToString()
    {
        var sb = new StringBuilder();

        foreach (var validationError in Error.ValidationErrors)
        {
            sb.Append(validationError.ValidationObjectType)
              .Append(' ')
              .Append(validationError.ObjectName)
              .Append(' ')
              .Append(validationError.ErrorCode)
              .Append("; ");
        }
        return sb.Length > 0
            ? sb.ToString().TrimEnd(' ', ';')
            : string.Empty;
    }
    public void AddValidationErrors(List<Error> errors)
    {
        foreach (var error in errors)
        {
            if (error != null)
                Error.ValidationErrors.AddRange(error.ValidationErrors);
        }
        if (Error.ValidationErrors.Count > 0)
            SetToFailure();
    }
}


public class Result<T> : Result
{
    public T? Data { get; set; }
    public Result<T> WithData(T? data)
    {
        Data = data;
        return this;
    }
    public static Result<T> Fail(Error error) => new() { IsSuccess = false, Error = error };

    public static Result<T> FailureWithData(T Object, Error error)
    {
        var result = Result<T>.Ok(Object);
        result.AddValidationError(error);
        return result;
    }

    public static implicit operator Result<T>(UnitResult result)
    {
        Result<T> tResult = new() { IsSuccess = result.IsSuccess };
        if (result.IsFailure)
            tResult.Error = result.Error;

        return tResult;
    }
}

