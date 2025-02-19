using static PetFamily.Domain.Shared.Validations.ValidationMessages;
using static PetFamily.Domain.Shared.Validations.ValidationCodes;

namespace PetFamily.Domain.DomainError;

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
    public static Error StringIsNullOrEmpty(string valueName) =>
        new(INVALID_VALUE_CODE,
            string.Format(INVALID_NULL_EMPTY, valueName),
            ErrorType.Validation,
            valueName);

    public static Error InvalidFormat(string valueName) =>
        new(INVALID_VALUE_CODE,
            string.Format(INVALID_FORMAT, valueName),
            ErrorType.Validation,
            valueName);

    public static Error InvalidLength(string valueName) =>
        new(INVALID_VALUE_CODE,
            string.Format(INVALID_LENGTH, valueName),
            ErrorType.Validation, valueName);

    public static Error ValueIsRequired(string valueName) =>
        new(INVALID_VALUE_CODE,
            string.Format(INVALID_REQUIRE_OPTION, valueName),
            ErrorType.Validation, valueName);

    public static Error GuidIsEmpty(string valueName) =>
        new(INVALID_VALUE_CODE,
            string.Format(INVALID_GUIDID_EMPTY, valueName),
            ErrorType.Validation,
            valueName);

    public static Error ValueIsBusy(string valueName) =>
      new(INVALID_VALUE_CODE,
          string.Format(VALUE_ALREADY_EXISTS, valueName),
          ErrorType.Validation, valueName);

    //-----------------------------------Other Errors---------------------------------------------//
    public static Error NotFound(string message) =>
        new("data.not.found", message, ErrorType.NotFound);

    public static Error Exception(Exception ex) =>
        new("exception", ex.ToString(), ErrorType.Exception);

    public static Error Custom(
        string code,
        string message,
        ErrorType errorType,
        string? valueName) =>
        new(code, message, errorType, valueName);

    public static Error InternalServerError(string message) =>
        new("internal.server.error", message, ErrorType.InternalServerError, string.Empty);

    public static Error Cancelled(string message) =>
        new("operation.is.cancelled", message, ErrorType.Cancellation, string.Empty);

}
