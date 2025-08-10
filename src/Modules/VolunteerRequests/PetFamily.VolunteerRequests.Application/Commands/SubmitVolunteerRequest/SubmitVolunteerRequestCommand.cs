using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;

public record SubmitVolunteerRequestCommand(
    string DocumentName,
    string LastName,
    string FirstName,
    string Description,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites) : ICommand;
