using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValueObjectValidations;
using static PetFamily.Auth.Domain.Validations.Validations;
using PetFamily.Auth.Domain.Options;
using PetFamily.SharedApplication.Exceptions;

namespace PetFamily.Auth.Application.UserManagement.Commands.RegisterByEmail;

public static class RegisterByEmailCommandValidator
{
    public static void Validate(RegisterByEmailCommand cmd)
    {
        var result = UnitResult.FromValidationResults(

            () => ValidateEmail(cmd.Email),

            () => ValidatePassword(cmd.Password),

            () => ValidateLogin(cmd.Login,LoginOptions.Default),

            () => ValidateRequiredPhone(cmd.phoneRegionCode, cmd.phoneNumber),

            () => ValidateItems(cmd.SocialNetworksList, s => SocialNetworkInfo.Validate(s.Name, s.Url))
        );

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }
}
