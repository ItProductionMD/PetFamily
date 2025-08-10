using PetFamily.Discussions.Domain.Entities;
using Xunit;
using System;

namespace TestPetFamilyDomain;

public class MessageTests
{
    [Fact]
    public void Create_ShouldSucceed_WithValidText()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var discussionId = Guid.NewGuid(); // Assuming discussionId is needed
        var text = "Hello world!";

        // Act
        var result = Message.Create(authorId, discussionId, text);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(text, result.Data!.Text);
        Assert.Equal(authorId, result.Data.AuthorId);
        Assert.Null(result.Data.EditedAt);
        Assert.True(result.Data.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_ShouldFail_WhenTextIsNull()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var discussionId = Guid.NewGuid(); // Assuming discussionId is needed
        string text = null!;

        // Act
        var result = Message.Create(authorId, discussionId, text);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_ShouldFail_WhenTextIsEmpty()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var discussionId = Guid.NewGuid(); // Assuming discussionId is needed
        var text = "";

        // Act
        var result = Message.Create(authorId, discussionId, text);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_ShouldFail_WhenTextExceedsMaxLength()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var discussionId = Guid.NewGuid(); // Assuming discussionId is needed
        var longText = new string('a', 10001); // assuming MAX_LENGTH_LONG_TEXT = 10000

        // Act
        var result = Message.Create(authorId, discussionId, longText);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Edit_ShouldSucceed_WithValidNewText()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var discussionId = Guid.NewGuid(); // Assuming discussionId is needed
        var message = Message.Create(authorId, discussionId, "Initial").Data!;

        // Act
        var result = message.Edit("Updated text");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated text", message.Text);
        Assert.NotNull(message.EditedAt);
        Assert.True(message.EditedAt!.Value >= message.CreatedAt);
    }

    [Fact]
    public void Edit_ShouldFail_WhenNewTextIsInvalid()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var discussionId = Guid.NewGuid(); // Assuming discussionId is needed
        var message = Message.Create(authorId, discussionId,  "Initial valid").Data!;

        // Act
        var result = message.Edit(""); // invalid empty text

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Initial valid", message.Text); // original text unchanged
        Assert.Null(message.EditedAt);
    }
}

