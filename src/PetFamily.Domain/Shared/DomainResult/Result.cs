namespace PetFamily.Domain.DomainResult
{
    public class Result
    {
        public bool IsValid { get; private set; }
        public bool IsFailure => !IsValid;
        public List<string> Errors { get; private set; } = [];

        private Result(bool isValid, List<string>? errors)
        {
            IsValid = isValid;
            if (errors != null)
                Errors = errors;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, [error]);
        public static Result Failure(List<string> errors) => new(false, errors);
        public static Result Failure() => new(false, null);
        public void AddError(string error)
        {
            IsValid = false;
            Errors.Add(error);
        }
    }
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public List<string> Errors { get; private set; } = [];
        public T Data { get; private set; }
        private Result()
        {
            Data = default!;
        }
        public static Result<T> Success(T result) => new() { IsSuccess = true, Data = result };
        public static Result<T> Failure(string error) => new() { IsSuccess = false, Errors = [error] };
        public static Result<T> Failure(List<string> errors) => new() { IsSuccess = false, Errors = errors };
        public void AddError(string error)
        {
            IsSuccess = false;
            Errors.Add(error);
        }
    }
}
