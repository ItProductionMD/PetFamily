using PetFamily.Domain.PetAggregates.Enums;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Domain.Shared.DTO;

public record PetDomainDto
{
    public string? Name { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateTime? DateTimeCreated { get; set; }
    public string? Description { get; set; }
    public bool? IsVaccinated { get; set; }
    public bool? IsSterilized { get; set; }
    public double Weight { get; set; }
    public double Height { get; set; }
    public string? Color { get; set; }
    public PetType? PetType { get; set; }
    public Phone? OwnerPhone { get; set; }
    public DonateDetails? DonateDetails { get; set; }
    public string? HealthInfo { get; set; }
    public Adress? Adress { get; set; }
    public HelpStatus HelpStatus { get; set; }
}
