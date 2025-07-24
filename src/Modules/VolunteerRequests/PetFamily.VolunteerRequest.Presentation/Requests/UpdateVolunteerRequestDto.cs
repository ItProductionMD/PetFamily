using PetFamily.SharedApplication.Dtos;

namespace PetFamily.VolunteerRequest.Presentation.Requests;

public record UpdateVolunteerRequestDto(
    string LastName,
    string FirstName,
    string Description,
    string DocumentName,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites);

