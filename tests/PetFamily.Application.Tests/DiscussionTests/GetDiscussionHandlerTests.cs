using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.Auth.Public.Contracts;
using PetFamily.Discussions.Application.Dtos;
using PetFamily.Discussions.Application.IRepositories;
using PetFamily.Discussions.Application.Queries.GetDiscussion;
using PetFamily.Discussions.Public.Dtos;
using PetFamily.SharedApplication.PaginationUtils;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.SharedApplication.Tests.DiscussionTests;


public class GetDiscussionHandlerTests
{
    private readonly Mock<IDiscussionReadRepository> _discussionRepo = new();
    private readonly Mock<ILogger<GetDiscussionHandler>> _logger = new();
    private readonly Mock<IParticipantContract> _participantContract = new();
    private readonly PaginationParams _paginationParams = new(1, 20);

    private GetDiscussionHandler CreateHandler() =>
        new(_discussionRepo.Object, _logger.Object, _participantContract.Object);

    private static DiscussionDto CreateDiscussionDto(Guid discussionId, List<Guid> participantIds)
        => new(
            Id: discussionId,
            IsClosed: false,
            participantIds,
            Messages: []
            );

    [Fact]
    public async Task Handle_Should_ReturnOk_When_UserIsParticipant()
    {
        // Arrange
        var ct = CancellationToken.None;
        var userId = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var discussionId = Guid.NewGuid();
        var discussionDto = CreateDiscussionDto(discussionId, [userId, userId2]);

        _discussionRepo.Setup(r => r.GetById(discussionId, It.IsAny<PaginationParams>(), ct))
            .ReturnsAsync(Result.Ok(discussionDto));

        var participantDtos = new List<ParticipantDto>
        {
            new(userId,"Test User", true )
        };

        _participantContract.Setup(p => p.GetByIds(It.IsAny<IReadOnlyList<Guid>>(), ct))
            .ReturnsAsync(Result.Ok(participantDtos));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetDiscussionQuery(
            userId,
            discussionId,
            _paginationParams.PageNumber, 
            _paginationParams.PageSize),
            ct);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(discussionDto.Id, result.Data!.DiscussionDto.Id);
    }

    [Fact]
    public async Task Handle_Should_ReturnForbidden_When_UserNotInParticipants()
    {
        // Arrange
        var ct = CancellationToken.None;
        var discussionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var userIdForbidden = Guid.NewGuid();
        var discussionDto = CreateDiscussionDto(discussionId, [adminId, userId]);

        _discussionRepo.Setup(r => r.GetById(discussionId, It.IsAny<PaginationParams>(), ct))
            .ReturnsAsync(Result.Ok(discussionDto));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetDiscussionQuery(userIdForbidden, discussionId, 1, 20), ct);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(Error.Forbidden("").Code, result.Error.Code);
    }

    [Fact]
    public async Task Handle_Should_ReturnFail_When_Discussion_NotFound()
    {
        // Arrange
        var ct = CancellationToken.None;
        var discussionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _discussionRepo.Setup(r => r.GetById(discussionId, It.IsAny<PaginationParams>(), ct))
            .ReturnsAsync(Result.Fail(Error.NotFound("Discussion not found")));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetDiscussionQuery(userId, discussionId, 1, 20), ct);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(Error.NotFound("").Code, result.Error.Code);
    }

    [Fact]
    public async Task Handle_Should_ReturnFail_When_ParticipantContractFails()
    {
        // Arrange
        var ct = CancellationToken.None;
        var userId = Guid.NewGuid();
        var discussionId = Guid.NewGuid();
        var discussionDto = CreateDiscussionDto(userId, [userId]);

        _discussionRepo.Setup(r => r.GetById(discussionId, It.IsAny<PaginationParams>(), ct))
            .ReturnsAsync(Result.Ok(discussionDto));

        _participantContract.Setup(p => p.GetByIds(It.IsAny<IReadOnlyList<Guid>>(), ct))
            .ReturnsAsync(Result.Fail(Error.InternalServerError("Service unavailable")));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetDiscussionQuery(userId, discussionId, 1, 20), ct);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(Error.InternalServerError("").Code, result.Error.Code);
    }
}
