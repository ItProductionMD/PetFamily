using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Discussions.Application.Commands.CreateDiscussion;

public record CreateDiscussionCommand(Guid AdminId, Guid UserId, Guid VolunteerRequestId) : ICommand;

