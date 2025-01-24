namespace PetFamily.Domain.Shared.DomainResult;

public  class Result
{
    private List<Error> _errors = [];
    public List<Error> Errors => _errors;
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;

    private Result() { }

    private void SetToFailure()
    {
        IsSuccess = false;
    }

    public void AddError(Error error)
    {
        _errors.Add(error);
        SetToFailure();
    }

    public void AddErrors(List<Error?> errors)
    {
        foreach (var error in errors)
        {
            if(error != null)
                _errors.Add(error);
        }
        if (_errors.Count > 0)
            SetToFailure();
    }

    public static Result Success() => new() { IsSuccess = true };

    public static Result Failure(Error error) 
    {
        var result = new Result();
        result.AddError(error);
        return result;
    }

    public static Result Failure(List<Error?> errors)
    {
        var result = new Result();
        result.AddErrors(errors);
        return result;
    }


    public static Result ValidateCollection(params Func<Result>[] validators)
    {
        var errors = new List<Error>();

        for (int i = 0; i < validators.Length; i++) 
        {
            var result = validators[i]();

            if (result.IsFailure && result.Errors!=null)
                errors.AddRange(result.Errors);           
        }
        return errors.Count > 0? Failure(errors): Success();
    }

    public static Result CreateFromErrors(List<Error> errors) =>

        errors.Count > 0 ? Failure(errors) : Success();

} 

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public T Data { get; init; }

    private List<Error> _errors = [];
    public List<Error> Errors => _errors;

    private Result() { Data = default!; }

    private void SetToFailure()
    {
        IsSuccess = false;
    }

    public void AddError(Error error)
    {
        _errors.Add(error);
        SetToFailure();
    }

    public void AddErrors(List<Error?> errors)
    {
        foreach (var error in errors)
        {
            if (error != null)
                _errors.Add(error);
        }
        if (_errors.Count > 0)
            SetToFailure();
    }

    public static Result<T> Success(T result) => new() { IsSuccess = true, Data = result };

    public static Result<T> Failure(Error error)
    {
        var result = new Result<T>();
        result.AddError(error);
        return result;
    }

    public static Result<T> Failure(List<Error?> errors)
    {
        var result = new Result<T>();
        result.AddErrors(errors);
        return result;
    }
}