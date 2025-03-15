using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.Commands.PetManagment.Dtos;

public record VolunteerDto
    (Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Description,
    string PhoneRegionCode,
    string PhoneNumber,
    int ExperienceYears,
    int PetsCount);
