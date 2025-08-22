using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Discussions.Application.Commands.DeleteDiscussionMessage;

public record DeleteDiscussionMessageCommand(Guid UserId, Guid DiscussionId, Guid MessageId) : ICommand;

