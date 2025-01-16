namespace PetFamily.Domain.Shared.DomainResult
{

    public class Result
    {
        public bool IsValid { get; init; }
        public bool IsFailure => !IsValid;
        public Error? Error { get; init; }

        private Result() { }

        public static Result Success() => new() { IsValid = true };

        public static Result Failure(Error error) => new() { IsValid = false, Error = error };

        public Result OnFailure(Func<Result> onFailureAction) => IsFailure ? this : onFailureAction();
    }

    public class Result<T>
    {
        public bool IsSuccess { get; init; }
        public bool IsFailure => !IsSuccess;
        public Error? Error { get; init; }
        public T Data { get; init; }

        private Result() { Data = default!; }

        public static Result<T> Success(T result) => new() { IsSuccess = true, Data = result };

        public static Result<T> Failure(Error error) => new() { IsSuccess = false, Error = error };

        public Result<T> OnFailure(Func<Result<T>> onFailureAction) =>

            IsFailure ? this : onFailureAction();
    }
}