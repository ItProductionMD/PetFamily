using PetFamily.Domain.DomainError;
using System.Runtime.CompilerServices;

namespace PetFamily.Domain.Results;
public abstract class Result
{
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    private List<Error> _errors = [];
    public List<Error> Errors => _errors;
    protected Result() { }
    public static Result<T> Ok<T>(T? data = default) => new() { IsSuccess = true, Data = data };
    public static Result<T> Ok<T>() => new() { IsSuccess = true };
    public static UnitResult Fail(Error error) => new() { IsSuccess = false, Errors = { error } };
    public static UnitResult Fail(List<Error> errors)
    {
        var result = new UnitResult();
        result.AddErrors(errors!);
        return result;
    }
    public string ConcateErrorMessages() => string.Join("; ", Errors.Select(e => e.Message));
    public void SetToFailure()
    {
        IsSuccess = false;
    }
    public void AddError(Error error)
    {
        _errors.Add(error);
        SetToFailure();
    }
    public void AddErrors(List<Error> errors)
    {
        foreach (var error in errors)
        {
            if (error != null)
                _errors.Add(error);
        }
        if (_errors.Count > 0)
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
    public Result<T> WithError(Error error)
    {
        AddError(error);
        return this;
    }
    public Result<T> WithErrorList(List<Error> errors)
    {
        AddErrors(errors);
        return this;
    }
    public static Result<T> Fail(Error error) => new() { IsSuccess = false, Errors = { error } };
    public static Result<T> Fail(List<Error> errors)
    {
        var result = new Result<T>();
        result.AddErrors(errors!);
        return result;
    }
    public static Result<T> FailureWithData(T Object, Error error)
    {
        var result = Result<T>.Ok(Object);
        result.AddError(error);
        return result;
    }

    public static implicit operator Result<T>(UnitResult result)
    {
        Result<T> tResult = new() { IsSuccess = result.IsSuccess };
        if (result.IsFailure)
            tResult.AddErrors(result.Errors);
        return tResult;
    }
}

