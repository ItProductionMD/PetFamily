using static PetFamily.Domain.Shared.Validations.ValidationMessages;
using static PetFamily.Domain.Shared.Validations.ValidationCodes;

namespace PetFamily.Domain.Shared;

public record Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public string? FieldName { get; }

    private Error(string errorCode, string message, ErrorType errorType, string? valueName = null)
    {
        Code = errorCode;
        Message = message;
        Type = errorType;
        FieldName = valueName;
    }
    //--------------------------Validation Errors(factory methods )-------------------------------//
    public static Error CreateErrorStringNullOrEmpty(string valueName) =>
        new(INVALID_VALUE_CODE,
            string.Format(INVALID_NULL_EMPTY, valueName),
            ErrorType.Validation,
            valueName);

    public static Error CreateErrorInvalidFormat(string valueName) =>
        new(INVALID_VALUE_CODE,
            string.Format(INVALID_FORMAT, valueName),
            ErrorType.Validation,
            valueName);

    public static Error CreateErrorInvalidLength(string valueName) =>
        new(INVALID_VALUE_CODE,
            string.Format(INVALID_LENGTH, valueName),
            ErrorType.Validation, valueName);

    public static Error CreateErrorValueRequired(string valueName) =>
        new(INVALID_VALUE_CODE,
            string.Format(INVALID_REQUIRE_OPTION, valueName),
            ErrorType.Validation, valueName);

    public static Error CreateErrorGuidIdIsEmpty(string valueName) =>
        new(INVALID_VALUE_CODE,
            string.Format(INVALID_GUIDID_EMPTY, valueName),
            ErrorType.Validation,
            valueName);

    public static Error CreateErrorValueIsBusy(string valueName) =>
      new(INVALID_VALUE_CODE,
          string.Format(VALUE_ALREADY_EXISTS, valueName),
          ErrorType.Validation, valueName);

    //-----------------------------------Other Errors---------------------------------------------//
    public static Error CreateErrorNotFound(string message) =>
        new("data.not.found", message, ErrorType.NotFound);

    public static Error CreateErrorException(Exception ex) =>
        new("exception", ex.ToString(), ErrorType.Exception);

    public static Error CreateCustomError(
        string code,
        string message,
        ErrorType errorType,
        string? valueName) =>
        new(code, message, errorType, valueName);

    public static Error CreateInternalServerError(string message) =>
        new("internal.server.error", message, ErrorType.InternalServerError, string.Empty);

}

public enum ErrorType
{
    NotFound,
    Validation,
    Conflict,
    Exception,
    InternalServerError,
}