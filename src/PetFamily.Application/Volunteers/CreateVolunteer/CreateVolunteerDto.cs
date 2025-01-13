namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerDto
{
    public string? FirstName { get;set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneRegionCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Street { get; set; }
    public string? ApartmentNumber { get; set; }
    public List<DonateDetailsDto>? DonateDetailsList { get; set; }

}
public class DonateDetailsDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
public class SocialNetworksDto
{
    public string? Name { get; set; }
    public string? Url { get; set; }
}