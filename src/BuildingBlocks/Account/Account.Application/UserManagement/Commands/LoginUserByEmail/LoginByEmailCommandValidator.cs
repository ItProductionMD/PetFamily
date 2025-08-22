using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using static Account.Domain.Validations.Validations;

namespace Account.Application.UserManagement.Commands.LoginUserByEmail;

public static class LoginByEmailCommandValidator
{
    public static void Validate(this LoginByEmailCommand cmd)
    {
        var result = UnitResult.FromValidationResults(
            () => ValidateEmail(cmd.Email),
            () => ValidatePassword(cmd.Password));

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }

}
