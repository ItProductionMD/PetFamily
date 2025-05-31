namespace Volunteers.Application.ResponseDtos;

public class PetWithVolunteerDto
{
    public Guid Id { get; set; }
    public string PetName { get; set; } = null!;
    public string? MainPhoto { get; set; }
    public string SpeciesName { get; set; } = null!;
    public string BreedName { get; set; } = null!;
    public string Color { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int AgeInMonths { get; set; }
    public string StatusForHelp { get; set; } = null!;
    public Guid VolunteerId { get; set; }
    public string VolunteerFullName { get; set; } = null!;
}
