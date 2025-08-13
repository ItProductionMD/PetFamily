using PetFamily.Auth.Application.UserManagement.Commands.RegisterByEmail;
using PetFamily.SharedApplication.Dtos;

namespace PetFamily.Auth.Presentation.Requests;

public class RegistrationByEmailRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string PhoneRegionCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public IEnumerable<SocialNetworksDto> SocialNetworks { get; set; } = [];

    public RegisterByEmailCommand ToCommand()
    {
        return new RegisterByEmailCommand(
            Email,
            Login,
            Password,
            PhoneRegionCode,
            PhoneNumber,
            SocialNetworks);
    }
}
