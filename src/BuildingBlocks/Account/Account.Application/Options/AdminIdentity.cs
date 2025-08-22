using PetFamily.SharedApplication.Abstractions;

namespace Account.Application.Options;

public class AdminIdentity : IApplicationOptions
{
    public const string SECTION_NAME = "AdminIdentity";
    public string Login { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;

    public static string GetSectionName() => SECTION_NAME;
}
