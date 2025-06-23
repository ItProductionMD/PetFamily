using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValueObjectValidations;
using static PetFamily.Auth.Domain.Validations.Validations;

namespace PetFamily.Auth.Application.UserManagement.Commands.LoginUserByEmail;

public static class LoginByEmailCommandValidator
{
    public static UnitResult Validate(LoginByEmailCommand cmd) =>
        UnitResult.FromValidationResults(
            () => ValidateEmail(cmd.Email),
            () => ValidatePassword(cmd.Password));

}
