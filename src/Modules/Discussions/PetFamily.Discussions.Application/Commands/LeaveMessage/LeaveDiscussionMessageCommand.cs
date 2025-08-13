using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Discussions.Application.Commands.LeaveMessage;

public record LeaveDiscussionMessageCommand(Guid UserId, Guid DiscussionId, string Message) : ICommand;
