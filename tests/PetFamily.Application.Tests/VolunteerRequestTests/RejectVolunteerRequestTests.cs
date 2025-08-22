using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Commands.RejectVolunteerRequest;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.Enums;

namespace PetFamily.SharedApplication.Tests.VolunteerRequestTests;

public class RejectVolunteerRequestTests
{
    private readonly Mock<IVolunteerRequestWriteRepository> _repo = new();
    private readonly Mock<ILogger<RejectVolunteerRequestHandler>> _logger = new();

    private RejectVolunteerRequestHandler CreateHandler()
        => new(_repo.Object, _logger.Object);

    private static VolunteerRequest CreateNewRequest(Guid userId)
    {
        var createResult = VolunteerRequest.Create(
            userId: userId,
            documentName: "doc.pdf",
            lastName: "Doe",
            firstName: "John",
            description: "Some desc",
            experienceYears: 2,
            requisites: []
        );
        Assert.True(createResult.IsSuccess);
        return createResult.Data!;
    }

    [Fact]
    public async Task Handle_Should_ReturnOk_And_Save_When_Request_Rejected_Successfully()
    {
        // arrange
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        var request = CreateNewRequest(userId);
        var discussionId = Guid.NewGuid();
        var takeRes = request.TakeToReview(adminId, discussionId);
        Assert.True(takeRes.IsSuccess);

        _repo.Setup(r => r.GetByIdAsync(requestId, ct))
             .ReturnsAsync(Result.Ok(request));

        _repo.Setup(r => r.SaveAsync(ct))
             .Returns(Task.CompletedTask);

        var handler = CreateHandler();
        var cmd = new RejectVolunteerRequestCommand(adminId, requestId, "Недостаточно данных");

        // act
        var result = await handler.Handle(cmd, ct);

        // assert
        Assert.True(result.IsSuccess);
        _repo.Verify(r => r.GetByIdAsync(requestId, ct), Times.Once);
        _repo.Verify(r => r.SaveAsync(ct), Times.Once);

        Assert.Equal(RequestStatus.Rejected, request.RequestStatus);
        Assert.Equal("Недостаточно данных", request.RejectedComment);
        Assert.NotNull(request.RejectedAt);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Request_Not_Found()
    {
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        _repo.Setup(r => r.GetByIdAsync(requestId, ct))
             .ReturnsAsync(Result.Fail(Error.NotFound("not found")));

        var handler = CreateHandler();
        var cmd = new RejectVolunteerRequestCommand(adminId, requestId, "any");

        var result = await handler.Handle(cmd, ct);

        Assert.True(result.IsFailure);
        _repo.Verify(r => r.SaveAsync(ct), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Request_Is_Not_OnReview()
    {
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        // Заявка ещё в Created — Reject запрещён
        var request = CreateNewRequest(userId);

        _repo.Setup(r => r.GetByIdAsync(requestId, ct))
             .ReturnsAsync(Result.Ok(request));

        var handler = CreateHandler();
        var cmd = new RejectVolunteerRequestCommand(adminId, requestId, "any");

        var result = await handler.Handle(cmd, ct);

        Assert.True(result.IsFailure);
        _repo.Verify(r => r.SaveAsync(ct), Times.Never);
        Assert.Equal(RequestStatus.Created, request.RequestStatus);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Another_Admin_Tries_To_Reject()
    {
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var adminWhoTook = Guid.NewGuid();
        var anotherAdmin = Guid.NewGuid();

        var request = CreateNewRequest(userId);
        var discussionId = Guid.NewGuid();
        var takeRes = request.TakeToReview(adminWhoTook, discussionId);
        Assert.True(takeRes.IsSuccess);

        _repo.Setup(r => r.GetByIdAsync(requestId, ct))
             .ReturnsAsync(Result.Ok(request));

        var handler = CreateHandler();
        var cmd = new RejectVolunteerRequestCommand(anotherAdmin, requestId, "any");

        var result = await handler.Handle(cmd, ct);

        Assert.True(result.IsFailure);
        _repo.Verify(r => r.SaveAsync(ct), Times.Never);
        Assert.Equal(RequestStatus.OnReview, request.RequestStatus);
    }
}
