using Microsoft.Extensions.Logging;
using Moq;
using Account.Public.Contracts;
using Account.Public.Dtos;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.VolunteerRequests.Application.Commands.ApproveVolunteerRequest;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.Enums;
using Volunteers.Public.Dto;
using Volunteers.Public.IContracts;

namespace PetFamily.SharedApplication.Tests.VolunteerRequestTests;

public class ApproveVolunteerRequestHandlerTests
{
    private readonly Mock<IVolunteerRequestWriteRepository> _requestRepo = new();
    private readonly Mock<IUserContract> _userFinder = new();
    private readonly Mock<IVolunteerCreator> _volunteerCreator = new();
    private readonly Mock<ILogger<ApproveVolunteerRequestHandler>> _logger = new();

    private ApproveVolunteerRequestHandler CreateHandler() =>
        new
        (
            _requestRepo.Object,
            _userFinder.Object,
            _volunteerCreator.Object,
            _logger.Object
        );

    private static VolunteerRequest CreateRequest(Guid userId, bool takenForReview, Guid adminId)
    {
        var create = VolunteerRequest.Create(
            userId: userId,
            documentName: "doc.pdf",
            lastName: "Doe",
            firstName: "John",
            description: "desc",
            experienceYears: 2,
            requisites: Array.Empty<RequisitesInfo>()
        );
        var req = create.Data!;

        if (takenForReview)
        {
            var effectiveAdmin = adminId;
            var discussionId = Guid.NewGuid();
            var take = req.TakeToReview(effectiveAdmin, discussionId);
            Assert.True(take.IsSuccess);
        }

        return req;
    }

    [Fact]
    public async Task Handle_Should_ReturnOk_When_All_Steps_Succeed()
    {
        // arrange
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = CreateRequest(userId, takenForReview: true, adminId: adminId);

        _requestRepo.Setup(r => r.GetByIdAsync(requestId, ct))
            .ReturnsAsync(Result.Ok(request));

        _requestRepo.Setup(r => r.SaveAsync(ct))
            .Returns(Task.Delay(0));

        UserDto userDto = new UserDto()
        {
            Id = userId,
            Email = "user@gmail.com",
            Login = "AUser",
            Phone = "+373000000"
        };

        _userFinder.Setup(f => f.GetByIdAsync(userId, ct))
            .ReturnsAsync(Result.Ok(userDto));

        _volunteerCreator.Setup(v => v.CreateVolunteer(It.IsAny<CreateVolunteerDto>(), ct))
            .ReturnsAsync(UnitResult.Ok());

        var handler = CreateHandler();

        // act
        var result = await handler.Handle(new ApproveVolunteerRequestCommand(adminId, requestId), ct);

        // assert
        Assert.True(result.IsSuccess);
        _requestRepo.Verify(r => r.SaveAsync(ct), Times.AtLeastOnce);
        _volunteerCreator.Verify(v => v.CreateVolunteer(
            It.Is<CreateVolunteerDto>(d =>
                d.UserId == userId &&
                d.FirstName == request.FirstName &&
                d.LastName == request.LastName &&
                d.ExperienceYears == request.ExperienceYears),
            ct), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Request_Not_Found()
    {
        // arrange
        var adminId = Guid.NewGuid();
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();

        _requestRepo.Setup(r => r.GetByIdAsync(requestId, ct))
            .ReturnsAsync(Result.Fail(Error.NotFound("not found")));

        var handler = CreateHandler();

        // act
        var result = await handler.Handle(new ApproveVolunteerRequestCommand(adminId, requestId), ct);

        // assert
        Assert.True(result.IsFailure);
        _requestRepo.Verify(r => r.SaveAsync(ct), Times.Never);
        _volunteerCreator.Verify(v => v.CreateVolunteer(It.IsAny<CreateVolunteerDto>(), ct), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Domain_Approve_Fails()
    {
        // arrange
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();
        var adminWhoTook = Guid.NewGuid();
        var adminTryingToApprove = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = CreateRequest(userId, takenForReview: true, adminId: adminWhoTook);

        _requestRepo.Setup(r => r.GetByIdAsync(requestId, ct))
            .ReturnsAsync(Result.Ok(request));

        var handler = CreateHandler();

        // act
        var result = await handler.Handle(
            new ApproveVolunteerRequestCommand(adminTryingToApprove, requestId),
            ct);

        // assert
        Assert.True(result.IsFailure);
        _requestRepo.Verify(r => r.SaveAsync(ct), Times.Never);
        _volunteerCreator.Verify(v => v.CreateVolunteer(It.IsAny<CreateVolunteerDto>(), ct), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Rollback_When_CreateVolunteer_Fails()
    {
        // arrange
        var ct = CancellationToken.None;
        var requestId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = CreateRequest(userId, takenForReview: true, adminId: adminId);

        _requestRepo.Setup(r => r.GetByIdAsync(requestId, ct))
            .ReturnsAsync(Result.Ok(request));

        _requestRepo.Setup(r => r.SaveAsync(ct))
            .Returns(Task.CompletedTask);

        var userDto = new UserDto()
        {
            Id = userId,
            Email = "user@gmail.com",
            Login = "AUser",
            Phone = "+373000000"
        };

        _userFinder.Setup(f => f.GetByIdAsync(userId, ct))
            .ReturnsAsync(Result.Ok(userDto));

        _volunteerCreator.Setup(v => v.CreateVolunteer(It.IsAny<CreateVolunteerDto>(), ct))
            .ReturnsAsync(UnitResult.Fail(Error.InternalServerError("create failed")));

        var handler = CreateHandler();

        // act
        var result = await handler.Handle(new ApproveVolunteerRequestCommand(adminId, requestId), ct);

        // assert
        Assert.True(result.IsFailure);

        _requestRepo.Verify(r => r.SaveAsync(ct), Times.AtLeast(2));
        Assert.Equal(RequestStatus.Created, request.RequestStatus);
    }
}
