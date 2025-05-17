namespace PetFamily.Infrastructure.Contexts.ReadDbContext.Models;

public partial class Pet
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public DateTime DateTimeCreated { get; set; }

    public string? Description { get; set; }

    public bool IsVaccinated { get; set; }

    public bool IsSterilized { get; set; }

    public double? Weight { get; set; }

    public double? Height { get; set; }

    public string? Color { get; set; }

    public string Requisites { get; set; } = null!;

    public string? HealthInfo { get; set; }

    public string HelpStatus { get; set; } = null!;

    public string Images { get; set; } = null!;

    public DateTime? DeletedDateTime { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? VolunteerId { get; set; }

    public string AddressCity { get; set; } = null!;

    public string AddressRegion { get; set; } = null!;

    public string AddressStreet { get; set; } = null!;

    public string OwnerPhoneNumber { get; set; } = null!;

    public string OwnerPhoneRegionCode { get; set; } = null!;

    public Guid PetTypeBreedId { get; set; }

    public Guid PetTypeSpeciesId { get; set; }

    public int SerialNumber { get; set; }

    public virtual Volunteer? Volunteer { get; set; }
}
