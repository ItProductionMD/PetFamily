using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace PetFamily.Discussion.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }

    private Message() { } //EFCORE need this

    private Message(Guid authorId, string text)
    {
        Id = Guid.NewGuid();
        AuthorId = authorId;
        Text = text;
        CreatedAt = DateTime.UtcNow;
        EditedAt = null;
    }

    public static Result<Message> Create(Guid authorId, string text)
    {
        var validateText = ValidateMessage(text);
        if (validateText.IsFailure)
            return validateText;

        return Result.Ok(new Message(authorId, text));
    }

    public UnitResult Edit(string newText)
    {
        var validateText = ValidateMessage(newText);
        if (validateText.IsFailure)
            return validateText;

        Text = newText;
        EditedAt = DateTime.UtcNow;
        return UnitResult.Ok();
    }

    public static UnitResult ValidateMessage(string text) =>
        ValidateRequiredField(text, "message", MAX_LENGTH_LONG_TEXT);   
}

