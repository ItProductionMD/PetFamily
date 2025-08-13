using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace PetFamily.Auth.Application.UserManagement.Commands.ChangeRoles;

public static class ChangeRoleValidator
{
    public static void Validate(ChangeRoleCommand cmd)
    {
        var result = UnitResult.FromValidationResults(
            () => ValidateIfGuidIsNotEpmty(cmd.RoleId, "RoleId"),
            () => ValidateIfGuidIsNotEpmty(cmd.UserId, "UserId"));

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }
}
