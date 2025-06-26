using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValueObjectValidations;
using static PetFamily.Auth.Domain.Validations.Validations;
using PetFamily.Auth.Domain.Options;

namespace PetFamily.Auth.Application.UserManagement.Commands.RegisterByEmail;

public static class RegisterByEmailCommandValidator
{
    public static UnitResult Validate(RegisterByEmailCommand cmd)
    {
        return UnitResult.FromValidationResults(

            () => ValidateEmail(cmd.Email),

            () => ValidatePassword(cmd.Password),

            () => ValidateLogin(cmd.Login,LoginOptions.Default),

            () => ValidatePhone(cmd.phoneRegionCode, cmd.phoneNumber),

            () => ValidateItems(cmd.SocialNetworksList, s => SocialNetworkInfo.Validate(s.Name, s.Url))
        );
    }
}
