using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValueObjectValidations;
using static PetFamily.Auth.Domain.Validations.Validations;
using PetFamily.SharedApplication.Exceptions;

namespace PetFamily.Auth.Application.UserManagement.Commands.LoginUserByEmail;

public static class LoginByEmailCommandValidator
{
    public static void Validate(LoginByEmailCommand cmd)
    {
        var result = UnitResult.FromValidationResults(
            () => ValidateEmail(cmd.Email),
            () => ValidatePassword(cmd.Password));

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }

}
