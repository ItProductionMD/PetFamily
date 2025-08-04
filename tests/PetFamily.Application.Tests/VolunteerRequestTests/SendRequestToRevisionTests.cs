using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Moq;
using PetFamily.Discussions.Public.Contracts;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.VolunteerRequests.Application.Commands.SendVolunteerRequestToRevision;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.Enums;

namespace PetFamily.Application.Tests.VolunteerRequestTests;

public class SendRequestToRevisionTests
{
    private readonly Mock<IVolunteerRequestWriteRepository> _repo = new();
    private readonly Mock<IUserContext> _userContext = new();
    private readonly Mock<IDiscussionMessageSender> _messageSender = new();
    private readonly Mock<ILogger<SendRequestToRevisionHandler>> _logger = new();

    private SendRequestToRevisionHandler CreateHandler() =>
        new(_repo.Object, _userContext.Object, _messageSender.Object, _logger.Object);

    private static VolunteerRequest CreateNewRequest(Guid userId)
    {
        var create = VolunteerRequest.Create(
            userId: userId,
            documentName: "doc.pdf",
            lastName: "Doe",
            firstName: "John",
            description: "Some desc",
            experienceYears: 3,
            requisites: Array.Empty<RequisitesInfo>() // подстрой под свою модель, если нужно
        );
        Assert.True(create.IsSuccess);
        return create.Data!;
    }

    [Fact]
    public async Task Handle_Should_ReturnOk_When_All_Steps_Succeed()
    {
        // arrange
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var discussionId = Guid.NewGuid();

        var request = CreateNewRequest(userId);
        var takeResult = request.TakeToReview(adminId, discussionId);
        if(takeResult.IsFailure)
        {
            throw new TestCanceledException("Take request to review Failed!");
        }
        _repo.Setup(r => r.GetByIdAsync(requestId, ct))
             .ReturnsAsync(Result.Ok(request));

        _userContext.Setup(c => c.GetUserId())
             .Returns(adminId);

        _repo.Setup(r => r.SaveAsync(ct))
             .Returns(Task.CompletedTask);

        _messageSender.Setup(s => s.Send(requestId, adminId, "Комментарий для доработки", ct))
             .ReturnsAsync(UnitResult.Ok());

        var handler = CreateHandler();
        var cmd = new SendRequestToRevisionCommand(requestId, "Комментарий для доработки");

        // act
        var result = await handler.Handle(cmd, ct);

        // assert
        Assert.True(result.IsSuccess);
        _repo.Verify(r => r.GetByIdAsync(requestId, ct), Times.Once);
        _repo.Verify(r => r.SaveAsync(ct), Times.AtLeastOnce);
        _messageSender.Verify(s => s.Send(requestId, adminId, "Комментарий для доработки", ct), Times.Once);

        Assert.Equal(RequestStatus.NeedsRevision, request.RequestStatus);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Request_Not_Found()
    {
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();

        _repo.Setup(r => r.GetByIdAsync(requestId, ct))
             .ReturnsAsync(Result.Fail(Error.NotFound("not found")));

        var handler = CreateHandler();
        var cmd = new SendRequestToRevisionCommand(requestId, "any");

        var result = await handler.Handle(cmd, ct);

        Assert.True(result.IsFailure);
        _repo.Verify(r => r.SaveAsync(ct), Times.Never);
        _messageSender.Verify(s => s.Send(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), ct), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_AdminId_Not_Available()
    {
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();
        var request = CreateNewRequest(Guid.NewGuid());

        _repo.Setup(r => r.GetByIdAsync(requestId, ct))
             .ReturnsAsync(Result.Ok(request));

        _userContext.Setup(c => c.TryGetUserId())
             .Returns(UnitResult.Fail(Error.InternalServerError("no user")));

        var handler = CreateHandler();
        var cmd = new SendRequestToRevisionCommand(requestId, "any");

        var result = await handler.Handle(cmd, ct);

        Assert.True(result.IsFailure);
        _repo.Verify(r => r.SaveAsync(ct), Times.Never);
        _messageSender.Verify(s => s.Send(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), ct), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Domain_SendBackToRevision_Fails()
    {
        // arrange: заявка в Created (не OnReview) → SendBackToRevision вернет Fail
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var request = CreateNewRequest(Guid.NewGuid());

        _repo.Setup(r => r.GetByIdAsync(requestId, ct))
             .ReturnsAsync(Result.Ok(request));

        _userContext.Setup(c => c.TryGetUserId())
             .Returns(Result.Ok(adminId));

        var handler = CreateHandler();
        var cmd = new SendRequestToRevisionCommand(requestId, "any");

        // act
        var result = await handler.Handle(cmd, ct);

        // assert
        Assert.True(result.IsFailure);
        _repo.Verify(r => r.SaveAsync(ct), Times.Never);
        _messageSender.Verify(s => s.Send(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), ct), Times.Never);
        Assert.Equal(RequestStatus.Created, request.RequestStatus);
    }

    [Fact]
    public async Task Handle_Should_Rollback_When_MessageSender_Fails()
    {
        // arrange
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = CreateNewRequest(userId);

        // Заявка должна быть в OnReview
        var discussionId = Guid.NewGuid();
        Assert.True(request.TakeToReview(adminId, discussionId).IsSuccess);

        _repo.Setup(r => r.GetByIdAsync(requestId, ct))
             .ReturnsAsync(Result.Ok(request));

        _userContext.Setup(c => c.TryGetUserId())
             .Returns(Result.Ok(adminId));

        // Save вызывается 1) после SendBackToRevision, 2) после CancelSendBackToRevision
        _repo.Setup(r => r.SaveAsync(ct))
             .Returns(Task.CompletedTask);

        // Сымитируем фейл отправки сообщения:
        _messageSender.Setup(s => s.Send(requestId, adminId, "bad", ct))
             .ReturnsAsync(UnitResult.Fail(Error.InternalServerError("send failed")));

        var handler = CreateHandler();
        var cmd = new SendRequestToRevisionCommand(requestId, "bad");

        // act
        var result = await handler.Handle(cmd, ct);

        // assert
        Assert.True(result.IsFailure);

        // SaveAsync должен быть вызван как минимум 2 раза: после изменения статуса и после отката
        _repo.Verify(r => r.SaveAsync(ct), Times.AtLeast(2));

        // После rollback статус возвращается к OnReview
        Assert.Equal(RequestStatus.OnReview, request.RequestStatus);
    }

    // Если у тебя IDiscussionMessageSender.Send возвращает Result<Guid>, то в тесте выше замени Setup на:
    //
    // _messageSender.Setup(s => s.Send(requestId, adminId, "bad", ct))
    //      .ReturnsAsync(Result.Fail<Guid>(Error.InternalServerError("send failed")));
}
