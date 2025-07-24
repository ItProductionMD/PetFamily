using PetFamily.Auth.Domain.Options;
using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace PetFamily.Auth.Domain.Validations;

public static class Validations
{
    public static UnitResult ValidateLogin(string login, LoginOptions loginOptions) =>
       ValidateRequiredField(
           login,
           "Login",
           loginOptions.maxLength,
           loginOptions.Regex);

    public static UnitResult ValidateEmail(string email) =>
        ValidateRequiredField(
            email,
            "Email",
            MAX_LENGTH_SHORT_TEXT,
            EMAIL_PATTERN);

    public static UnitResult ValidatePassword(string password) =>
        ValidateRequiredField(
            password,
            "Password",
            MAX_LENGTH_MEDIUM_TEXT);

}
