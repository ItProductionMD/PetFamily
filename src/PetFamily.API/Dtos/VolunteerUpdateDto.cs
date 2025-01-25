namespace PetFamily.API.Dtos
{
    public record VolunteerUpdateDto
    (
        string FirstName,
        string LastName,
        string Email,
        string Description,
        string PhoneNumber,
        string PhoneRegionCode,
        int ExperienceYears
    );
}
