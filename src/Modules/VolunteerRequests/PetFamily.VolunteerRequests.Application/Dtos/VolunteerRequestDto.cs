namespace PetFamily.VolunteerRequests.Application.Dtos;

public record VolunteerRequestDto(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName,
    string Description,
    int ExperienceYears,
    string DocumentName,
    DateTime CreatedAt,
    string RequestStatus
);
