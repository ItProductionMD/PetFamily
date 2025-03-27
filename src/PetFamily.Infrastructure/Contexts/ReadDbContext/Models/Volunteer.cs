using System;
using System.Collections.Generic;

namespace PetFamily.Infrastructure.Contexts.ReadDbContext.Models;

public partial class Volunteer
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public int ExperienceYears { get; set; }

    public string? Description { get; set; }

    public string Requisites { get; set; } = null!;

    public string SocialNetworks { get; set; } = null!;

    public DateTime? DeletedDateTime { get; set; }

    public bool IsDeleted { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string PhoneRegionCode { get; set; } = null!;

    public int Rating { get; set; }
    public string FullName => string.Join(' ', LastName, FirstName);
    public string Phone => string.Join('-', PhoneRegionCode, PhoneNumber);

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}
