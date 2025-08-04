using PetFamily.SharedApplication.Dtos;

namespace PetFamily.VolunteerRequests.Presentation.Requests;

public record SubmitVolunteerRequestDto(
    string DocumentName,
    string LastName,
    string FirstName,
    string Description,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites);
