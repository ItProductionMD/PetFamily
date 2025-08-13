using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Discussions.Application.Commands.CloseDiscussion;

public record CloseDiscussionCommand(Guid AdminId, Guid DiscussionId) : ICommand;

