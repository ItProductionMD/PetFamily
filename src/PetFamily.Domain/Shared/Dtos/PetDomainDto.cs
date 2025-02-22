using PetFamily.Domain.PetAggregates.Enums;
using PetFamily.Domain.PetAggregates.ValueObjects;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Domain.Shared.Dtos;
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
    IReadOnlyList<RequisitesInfo> DonateDetails,
    string? HealthInfo,
    Address? Adress,
    HelpStatus HelpStatus,
    List<Image> Images);

