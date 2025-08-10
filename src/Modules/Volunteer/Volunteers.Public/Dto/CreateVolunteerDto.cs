using PetFamily.SharedKernel.ValueObjects;

namespace Volunteers.Public.Dto;

public record CreateVolunteerDto(
    Guid UserId,
    string LastName,
    string FirstName,
    string Phone,
    int ExperienceYears,
    IEnumerable<RequisitesInfo> Requisites);
