using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Validations;

namespace Authorization.Application.PermissionManagement.Commands.AddPermissions;

public static class AddPermissionsValidator
{
    public static UnitResult Validate(this AddPermissionsCommand command)
    {
        var dtos = command.newPermissionCodes ?? [];
        var result = CheckForDuplicatedStrings(dtos);
        return result;
    }
    private static UnitResult CheckForDuplicatedStrings(List<string> items)
    {
        var duplicatedItems = items
            .GroupBy(p => p)
            .Where(g => g.Count() > 1)
            .ToList();

        var validationErrors = new List<ValidationError>();

        if (duplicatedItems.Any())
        {
            foreach (var group in duplicatedItems)
            {
                ValidationError validationError = new(
                    ValidationErrorType.Field,
                    $"Code for {group.Key}",
                    ValidationErrorCodes.VALUE_ALREADY_EXISTS);

                validationErrors.Add(validationError);
            }
        }

        return validationErrors.Count > 0
            ? UnitResult.Fail(Error.FromValidationErrors(validationErrors))
            : UnitResult.Ok();
    }

}
