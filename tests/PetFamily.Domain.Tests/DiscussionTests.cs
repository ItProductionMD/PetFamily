using PetFamily.Discussion.Domain.Entities;
using PetFamily.SharedKernel.Results;
using Xunit;

namespace TestPetFamilyDomain;

public class DiscussionTests
{
    [Fact]
    public void Create_ShouldSucceed_WhenTwoParticipantsProvided()
    {
        // Arrange
        var relationId = Guid.NewGuid();
        var participants = new[] { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        var result = Discussion.Create(relationId, participants);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(relationId, result.Data!.RelationId);
        Assert.Equal(2, result.Data.ParticipantIds.Count);
    }

    [Fact]
    public void Create_ShouldFail_WhenParticipantsCountIsNotTwo()
    {
        // Arrange
        var relationId = Guid.NewGuid();
        var participants = new[] { Guid.NewGuid() };

        // Act
        var result = Discussion.Create(relationId, participants);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void LeaveMessage_ShouldSucceed_ForParticipant()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), new[] { user1, user2 }).Data!;

        // Act
        var result = discussion.LeaveMessage(user1, "Hello there");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(discussion.Message);
        Assert.Equal("Hello there", result.Data!.Text);
    }

    [Fact]
    public void LeaveMessage_ShouldFail_ForNonParticipant()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var outsider = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), new[] { user1, user2 }).Data!;

        // Act
        var result = discussion.LeaveMessage(outsider, "I shouldn't be here");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Empty(discussion.Message);
    }

    [Fact]
    public void LeaveMessage_ShouldFail_WhenDiscussionClosed()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), new[] { user1, Guid.NewGuid() }).Data!;
        discussion.Close();

        // Act
        var result = discussion.LeaveMessage(user1, "Too late");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void EditMessage_ShouldSucceed_ByAuthor()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), new[] { user1, Guid.NewGuid() }).Data!;
        var message = discussion.LeaveMessage(user1, "Original").Data!;

        // Act
        var result = discussion.EditMessage(user1, message.Id, "Updated");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", discussion.Message.First().Text);
        Assert.NotNull(discussion.Message.First().EditedAt);
    }

    [Fact]
    public void EditMessage_ShouldFail_IfNotAuthor()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), new[] { user1, user2 }).Data!;
        var message = discussion.LeaveMessage(user1, "Private note").Data!;

        // Act
        var result = discussion.EditMessage(user2, message.Id, "Hacked!");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Private note", discussion.Message.First().Text);
    }

    [Fact]
    public void DeleteMessage_ShouldSucceed_ByAuthor()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), new[] { user1, Guid.NewGuid() }).Data!;
        var message = discussion.LeaveMessage(user1, "To be removed").Data!;

        // Act
        var result = discussion.DeleteMessage(user1, message.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(discussion.Message);
    }

    [Fact]
    public void DeleteMessage_ShouldFail_IfNotAuthor()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), new[] { user1, user2 }).Data!;
        var message = discussion.LeaveMessage(user1, "Don't touch this").Data!;

        // Act
        var result = discussion.DeleteMessage(user2, message.Id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Single(discussion.Message);
    }

    [Fact]
    public void Close_ShouldPreventNewMessages()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var discussion = Discussion.Create(Guid.NewGuid(), new[] { user1, Guid.NewGuid() }).Data!;
        discussion.Close();

        // Act
        var result = discussion.LeaveMessage(user1, "After closed");

        // Assert
        Assert.True(result.IsFailure);
    }
}

