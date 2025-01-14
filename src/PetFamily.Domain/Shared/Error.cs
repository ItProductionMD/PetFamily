using static PetFamily.Domain.Shared.Validations.ValidationMessages;

namespace PetFamily.Domain.Shared
{

    public record Error
    {
        public string Code { get; }
        public string Message { get; }
        public ErrorType Type { get; }

        private Error(string code, string message, ErrorType errorType)
        {
            Code = code;
            Message = message;
            Type = errorType;
        }

        public static Error CreateErrorStringNullOrEmpty(string valueName) =>

            new("value.is.invalid", string.Format(INVALID_NULL_EMPTY, valueName), ErrorType.Validation);

        public static Error CreateErrorInvalidFormat(string valueName) =>

            new("value.is.invalid", string.Format(INVALID_FORMAT, valueName), ErrorType.Validation);

        public static Error CreateErrorInvalidLength(string valueName) =>

            new("value.is.invalid", string.Format(INVALID_LENGTH, valueName), ErrorType.Validation);

        public static Error CreateErrorValueRequired(string valueName) =>

            new("value.is.invalid", string.Format(INVALID_REQUIRE_OPTION, valueName), ErrorType.Validation);

        public static Error CreateErrorGuidIdIsEmpty(string valueName) =>

            new("value.is.invalid", string.Format(INVALID_GUIDID_EMPTY, valueName), ErrorType.Validation);

        public static Error CreateErrorConflict(string code, string message) =>

          new(code, message, ErrorType.Conflict);

        public static Error CreateErrorNotFound(string code, string message) =>

            new(code, message, ErrorType.NotFound);

        public static Error CreateErrorException(Exception ex) =>

            new("exception", ex.ToString(), ErrorType.Exception);

    }

    public enum ErrorType
    {
        NotFound,
        Validation,
        Conflict,
        Exception
    }
}