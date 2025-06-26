using Org.BouncyCastle.Cms;

namespace PetFamily.Auth.Domain.Options;

public class LoginOptions
{
    public int minLength { get; set; } = 3;
    public int maxLength { get; set; } = 20;
    public string Regex { get; set; } = "^[a-zA-Z0-9_]+$"; // Example regex for alphanumeric and underscore
    public bool AllowSpecialCharacters { get; set; } = false;

    public static LoginOptions Default =>
        new LoginOptions
        {
            minLength = 3,
            maxLength = 20,
            Regex = "^[a-zA-Z0-9_]+$",
            AllowSpecialCharacters = false
        };

}
