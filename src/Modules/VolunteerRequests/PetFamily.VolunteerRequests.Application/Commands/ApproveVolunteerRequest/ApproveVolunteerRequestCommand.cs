using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.VolunteerRequests.Application.Commands.ApproveVolunteerRequest;

public record ApproveVolunteerRequestCommand(Guid VolunteerRequestId) : ICommand;

