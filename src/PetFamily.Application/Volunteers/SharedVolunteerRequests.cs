namespace PetFamily.Application.Volunteers;

public static class SharedVolunteerRequests
{
    public record SocialNetworksRequest(string Name,string Url);

    public record RequisitesRequest(string Name, string Description);

}
