namespace PetFamily.SharedKernel.Validations;

public record ValidationError(
    ValidationErrorType ValidationObjectType,
    string ObjectName,
    string ErrorCode)
{
    public string ToErrorMessage() => ObjectName + " " + ErrorCode;
}
