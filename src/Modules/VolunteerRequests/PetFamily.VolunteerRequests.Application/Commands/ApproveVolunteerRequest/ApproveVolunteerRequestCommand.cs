using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.VolunteerRequests.Application.Commands.ApproveVolunteerRequest;

public record ApproveVolunteerRequestCommand(Guid AdminId, Guid VolunteerRequestId) : ICommand;

