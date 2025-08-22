using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Domain.Entities;

public class Discussion : SoftDeletable, IEntity<Guid>
{
    private readonly List<Message> _messages = new();
    public Guid Id { get; private set; }
    public Guid RelationId { get; private set; }
    public bool IsClosed { get; private set; }

    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
    public IReadOnlyList<Guid> ParticipantIds { get; private set; } = [];

    private Discussion() { }

    private Discussion(Guid relationId, IEnumerable<Guid> participantIds)
    {
        Id = Guid.NewGuid();
        RelationId = relationId;
        ParticipantIds = participantIds.ToList();
        IsClosed = false;
    }

    public static Result<Discussion> Create(Guid relationId, Guid AdminId, Guid UserId)
    {
        var participantIds = new List<Guid>() { AdminId, UserId };
        return Result.Ok(new Discussion(relationId, participantIds));
    }

    public Result<Message> LeaveMessage(Guid authorId, string text)
    {
        if (IsClosed)
            return Result.Fail(Error.InternalServerError("Discussion is closed."));

        if (!ParticipantIds.Contains(authorId))
            return Result.Fail(Error.InternalServerError("User is not a participant of the discussion."));

        var messageResult = Entities.Message.Create(authorId, Id, text);
        if (messageResult.IsFailure)
            return messageResult;

        var message = messageResult.Data!;
        _messages.Add(message);

        return Result.Ok(message);
    }

    public UnitResult EditMessage(Guid authorId, Guid messageId, string newText)
    {
        var message = _messages.FirstOrDefault(c => c.Id == messageId);
        if (message == null)
            return UnitResult.Fail(Error.InternalServerError("Message not found."));

        if (message.AuthorId != authorId)
            return Result.Fail(Error.InternalServerError("User can only edit their own messages."));

        var editResult = message.Edit(newText);
        if (editResult.IsFailure)
            return editResult;

        return UnitResult.Ok();
    }

    public UnitResult DeleteMessage(Guid authorId, Guid messageId)
    {
        var message = _messages.FirstOrDefault(c => c.Id == messageId);
        if (message == null)
            return UnitResult.Fail(Error.InternalServerError("Message not found."));

        if (message.AuthorId != authorId)
            return Result.Fail(Error.InternalServerError("User can only delete their own messages."));

        _messages.Remove(message);

        return UnitResult.Ok();
    }

    public UnitResult Close(Guid adminId)
    {
        if (!ParticipantIds.Contains(adminId))
            return UnitResult.Fail(Error.InternalServerError("User is not a participant of the discussion."));

        if (IsClosed)
            return UnitResult.Fail(Error.Conflict($"Discussion with Id:{Id} is already has been closed!"));

        IsClosed = true;
        return UnitResult.Ok();
    }
}

