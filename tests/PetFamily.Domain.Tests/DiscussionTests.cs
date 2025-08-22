using PetFamily.Discussions.Domain.Entities;

namespace TestPetFamilyDomain;

public class DiscussionTests
{
    [Fact]
    public void Create_ShouldSucceed_WhenTwoParticipantsProvided()
    {
        // Arrange
        var relationId = Guid.NewGuid();

        // Act
        var result = Discussion.Create(relationId, Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(relationId, result.Data!.RelationId);
        Assert.Equal(2, result.Data.ParticipantIds.Count);
    }

    [Fact]
    public void LeaveMessage_ShouldSucceed_ForParticipant()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), user1, user2).Data!;

        // Act
        var result = discussion.LeaveMessage(user1, "Hello there");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(discussion.Messages);
        Assert.Equal("Hello there", result.Data!.Text);
    }


    [Fact]
    public void LeaveMessage_ShouldFail_WhenDiscussionClosed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), userId, Guid.NewGuid()).Data!;
        discussion.Close(userId);

        // Act
        var result = discussion.LeaveMessage(userId, "Too late");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void EditMessage_ShouldSucceed_ByAuthor()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), user1, Guid.NewGuid()).Data!;
        var message = discussion.LeaveMessage(user1, "Original").Data!;

        // Act
        var result = discussion.EditMessage(user1, message.Id, "Updated");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", discussion.Messages.First().Text);
        Assert.NotNull(discussion.Messages.First().EditedAt);
    }

    [Fact]
    public void EditMessage_ShouldFail_IfNotAuthor()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), user1, user2).Data!;
        var message = discussion.LeaveMessage(user1, "Private note").Data!;

        // Act
        var result = discussion.EditMessage(user2, message.Id, "Hacked!");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Private note", discussion.Messages.First().Text);
    }

    [Fact]
    public void DeleteMessage_ShouldSucceed_ByAuthor()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), user1, Guid.NewGuid()).Data!;
        var message = discussion.LeaveMessage(user1, "To be removed").Data!;

        // Act
        var result = discussion.DeleteMessage(user1, message.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(discussion.Messages);
    }

    [Fact]
    public void DeleteMessage_ShouldFail_IfNotAuthor()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), user1, user2).Data!;
        var message = discussion.LeaveMessage(user1, "Don't touch this").Data!;

        // Act
        var result = discussion.DeleteMessage(user2, message.Id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Single(discussion.Messages);
    }

    [Fact]
    public void Close_ShouldPreventNewMessages()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), userId, Guid.NewGuid()).Data!;
        discussion.Close(userId);

        // Act
        var result = discussion.LeaveMessage(userId, "After closed");

        // Assert
        Assert.True(result.IsFailure);
    }
}

