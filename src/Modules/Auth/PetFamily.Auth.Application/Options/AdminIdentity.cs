using System.ComponentModel.DataAnnotations;
namespace PetFamily.Auth.Application.AdminOptions;

public class AdminIdentity
{
    public const string SectionName = "AdminIdentity";
    public string Login { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;
}
