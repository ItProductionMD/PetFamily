using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.Volunteers.Dtos;

public record VolunteerDto
    (Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Description,
    string PhoneRegionCode,
    string PhoneNumber,
    int ExperienceYears,
    IEnumerable<RequisitesInfo> Requisites,
    IEnumerable<SocialNetworkInfo> SocialNetworks,
    int PetsCount);
