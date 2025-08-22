namespace Account.Domain.Options;

public class PasswordOptions
{
    public int minLength { get; set; } = 6;
    public int maxLength { get; set; } = 100;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireDigit { get; set; } = true;
    public bool RequireNonAlphanumeric { get; set; } = false;
}
