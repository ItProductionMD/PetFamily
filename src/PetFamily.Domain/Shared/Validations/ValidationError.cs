namespace PetFamily.Domain.Shared.Validations;

public record ValidationError(
    ValidationErrorType ValidationObjectType,
    string ObjectName,
    string ErrorCode)
{
    public string ToErrorMessage() => ObjectName + " " + ErrorCode;
}
