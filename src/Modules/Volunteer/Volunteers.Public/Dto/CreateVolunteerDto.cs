using PetFamily.SharedKernel.ValueObjects;

namespace Volunteers.Public.Dto;

public record CreateVolunteerDto(
    Guid AdminId,
    Guid UserId,
    string LastName,
    string FirstName,
    string Phone,
    int ExperienceYears,
    IEnumerable<RequisitesInfo> Requisites);
