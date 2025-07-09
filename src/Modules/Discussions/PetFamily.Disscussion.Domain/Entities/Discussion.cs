using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussion.Domain.Entities;

public class Discussion
{
    private readonly List<Message> _message = new();
    private readonly List<Guid> _participantIds = new();

    public Guid Id { get; private set; }
    public Guid RelationId { get; private set; }
    public bool IsClosed { get; private set; }

    public IReadOnlyCollection<Message> Message => _message.AsReadOnly();
    public IReadOnlyCollection<Guid> ParticipantIds => _participantIds.AsReadOnly();

    private Discussion() { }

    private Discussion(Guid relationId, IEnumerable<Guid> participantIds)
    {
        Id = Guid.NewGuid();
        RelationId = relationId;
        _participantIds.AddRange(participantIds);
        IsClosed = false;
    }

    public static Result<Discussion> Create(Guid relationId, IEnumerable<Guid> participantIds)
    {
        if (participantIds == null || participantIds.Count() != 2)
            return Result.Fail(Error.InternalServerError("Discussion must have exactly 2 participants."));

        return Result.Ok(new Discussion (relationId, participantIds));     
    }

    public Result<Message> LeaveMessage(Guid authorId, string text)
    {
        if (IsClosed)
            return Result.Fail(Error.InternalServerError("Discussion is closed."));

        if (!_participantIds.Contains(authorId))
            return Result.Fail(Error.InternalServerError("User is not a participant of the discussion."));

        var messageResult = Entities.Message.Create(authorId, text);
        if(messageResult.IsFailure)
            return messageResult;

        var message = messageResult.Data!;
        _message.Add(message);

        return Result.Ok(message);
    }

    public UnitResult EditMessage(Guid authorId, Guid messageId, string newText)
    {
        var message = _message.FirstOrDefault(c => c.Id == messageId);
        if (message == null)
            return UnitResult.Fail(Error.InternalServerError("Message not found."));

        if (message.AuthorId != authorId)
            return Result.Fail(Error.InternalServerError("User can only edit their own messages."));

        var editResult = message.Edit(newText);
        if(editResult.IsFailure)
            return editResult;

        return UnitResult.Ok();
    }

    public UnitResult DeleteMessage(Guid authorId, Guid messageId)
    {
        var message = _message.FirstOrDefault(c => c.Id == messageId);
        if (message == null)
            return UnitResult.Fail(Error.InternalServerError("Message not found."));

        if (message.AuthorId != authorId)
            return Result.Fail(Error.InternalServerError("User can only delete their own messages."));

        _message.Remove(message);

        return UnitResult.Ok();
    }

    public void Close()
    {
        IsClosed = true;
    }
}

