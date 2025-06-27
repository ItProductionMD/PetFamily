namespace PetFamily.Auth.Presentation.Requests;

public class RefreshTokensRequest()
{
    public string FingerPrint { get; set; }
    public string AccessToken { get; set; }
}