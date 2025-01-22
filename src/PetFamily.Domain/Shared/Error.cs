using static PetFamily.Domain.Shared.Validations.ValidationMessages;

namespace PetFamily.Domain.Shared;


public record Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public string? FieldName { get; }

    private Error(string code, string message, ErrorType errorType, string? valueName)
    {
        Code = code;
        Message = message;
        Type = errorType;
        FieldName = valueName;
    }

    public static Error CreateErrorStringNullOrEmpty(string valueName) =>

        new("value.is.invalid", string.Format(INVALID_NULL_EMPTY, valueName), ErrorType.Validation, valueName);

    public static Error CreateErrorInvalidFormat(string valueName) =>

        new("value.is.invalid", string.Format(INVALID_FORMAT, valueName), ErrorType.Validation, valueName);

    public static Error CreateErrorInvalidLength(string valueName) =>

        new("value.is.invalid", string.Format(INVALID_LENGTH, valueName), ErrorType.Validation, valueName);

    public static Error CreateErrorValueRequired(string valueName) =>

        new("value.is.invalid", string.Format(INVALID_REQUIRE_OPTION, valueName), ErrorType.Validation, valueName);

    public static Error CreateErrorGuidIdIsEmpty(string valueName) =>

        new("value.is.invalid", string.Format(INVALID_GUIDID_EMPTY, valueName), ErrorType.Validation, valueName);

    public static Error CreateErrorConflict(string valueName) =>

      new( "data.is.conficted","" ,ErrorType.Conflict, valueName: null);

    public static Error CreateErrorNotFound(string code, string message) =>

        new(code, message, ErrorType.NotFound, valueName: null);

    public static Error CreateErrorException(Exception ex) =>

        new("exception", ex.ToString(), ErrorType.Exception, valueName: null);

    public static Error CreateCustomError(string code, string message, ErrorType errorType, string? valueName)
    {
        return new(code, message, errorType, valueName);
    }

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