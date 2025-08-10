using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.VolunteerRequests.Application.Commands.UpdateVolunteerRequest;

public record UpdateVolunteerRequestCommand(
    Guid VolunteerRequestId,
    string LastName,
    string FirstName,
    string Description,
    string DocumentName,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites) : ICommand;


