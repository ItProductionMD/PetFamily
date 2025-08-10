namespace PetFamily.Auth.Presentation.Requests;

public class LoginByEmailRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FingerPrint {  get; set; } = string.Empty;
}
