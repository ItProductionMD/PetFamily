using PetFamily.SharedKernel.Validations;

namespace PetFamily.SharedKernel.Errors;

public record Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public List<ValidationError> ValidationErrors { get; } = [];

    private Error(
        string errorCode,
        string message,
        ErrorType errorType,
        List<ValidationError>? validationErrors = null)
    {
        Code = errorCode;
        Message = message;
        Type = errorType;
        ValidationErrors = validationErrors ?? [];
    }
    public void AddValidationError(ValidationError validationError)
    {
        ValidationErrors.Add(validationError);
    }
    public void AddValidationErrors(List<ValidationError> validationErrors)
    {
        ValidationErrors.AddRange(validationErrors);
    }
    //--------------------------------------Validation Errors-------------------------------------//
    public static Error ValidationError(List<ValidationError> validationErrors) =>
        new(ErrorCodes.VALIDATION_ERROR,
            string.Empty,
            ErrorType.Validation,
            validationErrors);
    public static Error StringIsNullOrEmpty(string valueName) =>
        new(ErrorCodes.VALIDATION_ERROR,
            string.Empty,
            ErrorType.Validation,
            [new(ValidationErrorType.Field, valueName, ValidationErrorCodes.VALUE_IS_EMPTY)]);

    public static Error InvalidFormat(string valueName) =>
        new(ErrorCodes.VALIDATION_ERROR,
            string.Empty,
            ErrorType.Validation,
            [new(ValidationErrorType.Field, valueName, ValidationErrorCodes.VALUE_INVALID_FORMAT)]);

    public static Error InvalidLength(string valueName) =>
        new(ErrorCodes.VALIDATION_ERROR,
            string.Empty,
            ErrorType.Validation,
            [new(ValidationErrorType.Field, valueName, ValidationErrorCodes.VALUE_INVALID_LENGTH)]);

    public static Error GuidIsEmpty(string valueName) =>
        new(ErrorCodes.VALIDATION_ERROR,
            string.Empty,
            ErrorType.Validation,
            [new(ValidationErrorType.Field, valueName, ValidationErrorCodes.VALUE_IS_EMPTY)]);

    public static Error ValueIsAlreadyExist(string valueName) =>
      new(ErrorCodes.VALIDATION_ERROR,
          "value already exist",
          ErrorType.Validation,
          [new(ValidationErrorType.Field, valueName, ValidationErrorCodes.VALUE_ALREADY_EXISTS)]);

    public static Error ValuesAreAlreadyExist(List<ValidationError> validationErrors) =>
      new(ErrorCodes.VALIDATION_ERROR,
          string.Empty,
          ErrorType.Validation,
          validationErrors);
    //----------------------------------------File errors-----------------------------------------//
    public static Error FileValidation(List<Error> errors) =>
        new(ErrorCodes.VALIDATION_ERROR,
          string.Empty,
          ErrorType.Validation,
          errors.SelectMany(e => e.ValidationErrors).ToList());

    public static Error FileValidation(string fileName, string validationCode) =>
        new(ErrorCodes.VALIDATION_ERROR,
          string.Empty,
          ErrorType.Validation,
          [new(ValidationErrorType.File, fileName, validationCode)]);

    //-----------------------------------------Other Errors---------------------------------------//
    public static Error NotFound(string message) =>
        new(ErrorCodes.DATA_NOT_FOUND, message, ErrorType.NotFound);

    public static Error Custom(
        string code,
        string message,
        ErrorType errorType
        ) => new(code, message, errorType);

    public static Error InternalServerError(string message) =>
        new(ErrorCodes.INTERNAL_SERVER_ERROR, message, ErrorType.InternalServerError);

    public static Error Cancelled(string message) =>
        new(ErrorCodes.OPERATION_CANCELLED, message, ErrorType.Cancellation);

    public static Error ValueOutOfRange(string message) =>
        new(ErrorCodes.VALIDATION_ERROR,
            message,
            ErrorType.Validation,
            [new(ValidationErrorType.Field, message, ValidationErrorCodes.VALUE_OUT_OF_RANGE)]);


    public static Error FilesCountIsNull() =>
       new(ErrorCodes.VALIDATION_ERROR,
           "File list to handle is empty!",
           ErrorType.Validation,
           [new(ValidationErrorType.File, "count", ValidationErrorCodes.FILES_COUNT_IS_NULL)]);

    public static Error Conflict(string message) =>
        new(ErrorCodes.CONFLICT_ERROR,
            message,
            ErrorType.Conflict);
}

