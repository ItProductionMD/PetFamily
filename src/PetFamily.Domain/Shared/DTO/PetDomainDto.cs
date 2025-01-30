using PetFamily.Domain.PetAggregates.Enums;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Domain.Shared.DTO;
public record PetDomainDto(
    string? Name,
    DateOnly? DateOfBirth,
    string? Description,
    bool? IsVaccinated,
    bool? IsSterilized,
    double Weight,
    double Height,
    string? Color,
    PetType? PetType,
    Phone? OwnerPhone,
    DonateDetails? DonateDetails,
    string? HealthInfo,
    Address? Adress,
    HelpStatus HelpStatus);

