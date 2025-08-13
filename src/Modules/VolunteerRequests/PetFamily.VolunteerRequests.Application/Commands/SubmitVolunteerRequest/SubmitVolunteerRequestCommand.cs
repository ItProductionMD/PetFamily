using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;

namespace PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;

public record SubmitVolunteerRequestCommand(
    Guid UserId,
    string DocumentName,
    string LastName,
    string FirstName,
    string Description,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites) : ICommand;
