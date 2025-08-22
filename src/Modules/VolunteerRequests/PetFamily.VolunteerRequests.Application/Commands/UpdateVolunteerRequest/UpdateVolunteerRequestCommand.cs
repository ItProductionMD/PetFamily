using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;

namespace PetFamily.VolunteerRequests.Application.Commands.UpdateVolunteerRequest;

public record UpdateVolunteerRequestCommand(
    Guid UserId,
    Guid VolunteerRequestId,
    string LastName,
    string FirstName,
    string Description,
    string DocumentName,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites) : ICommand;


