using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Discussions.Application.Commands.UpdateMessage;

public record UpdateDiscussionMessageCommand(Guid UserId, Guid DiscussionId, Guid MessageId, string NewMessage) :ICommand;

