using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Entities;
using Xunit.Sdk;

namespace PetFamily.Application.Tests.VolunteerRequestTests;

public class SubmitVolunteerRequestTests
{
    private readonly Mock<IUserContext> _userContext = new();
    private readonly Mock<IVolunteerRequestWriteRepository> _writeRepo = new();
    private readonly Mock<IVolunteerRequestReadRepository> _readRepo = new();
    private readonly Mock<ILogger<SubmitVolunteerRequestHandler>> _logger = new();

    private SubmitVolunteerRequestHandler CreateHandler()
        => new(_userContext.Object, _writeRepo.Object, _readRepo.Object, _logger.Object);

    private static SubmitVolunteerRequestCommand CreateValidCommand()
    {
        return new SubmitVolunteerRequestCommand(
            DocumentName: "doc.pdf",
            LastName: "Doe",
            FirstName: "John",
            Description: "I want to help animals",
            ExperienceYears: 2,
            Requisites: []
        );
    }

    [Fact]
    public async Task Handle_Should_ReturnOk_And_Save_When_Valid_And_Not_Exists()
    {
        // arrange
        var ct = CancellationToken.None;
        var cmd = CreateValidCommand();
        var userId = Guid.NewGuid();

        _userContext.Setup(x => x.GetUserId())
            .Returns(userId);

        _readRepo.Setup(x => x.CheckIfRequestExistAsync(userId, ct))
            .ReturnsAsync(false);

        _writeRepo.Setup(x => x.AddAsync(It.IsAny<VolunteerRequest>(), ct))
            .Returns(Task.CompletedTask);

        _writeRepo.Setup(x => x.SaveAsync(ct))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        // act
        var result = await handler.Handle(cmd, ct);

        // assert
        Assert.True(result.IsSuccess);

        _readRepo.Verify(x => x.CheckIfRequestExistAsync(userId, ct), Times.Once);
        _writeRepo.Verify(x => x.AddAsync(It.Is<VolunteerRequest>(vr =>
                vr.UserId == userId &&
                vr.DocumentName == cmd.DocumentName &&
                vr.FirstName == cmd.FirstName &&
                vr.LastName == cmd.LastName &&
                vr.Description == cmd.Description &&
                vr.ExperienceYears == cmd.ExperienceYears &&
                vr.Requisites != null &&
                vr.Requisites.Count == cmd.Requisites.Count()
            ), ct), Times.Once);

        _writeRepo.Verify(x => x.SaveAsync(ct), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_Validation_Fails()
    {
        // arrange
        var ct = CancellationToken.None;

        var invalidCmd = new SubmitVolunteerRequestCommand(
            DocumentName: "",           
            LastName: "Doe",
            FirstName: "",             
            Description: new string('a', 5000), 
            ExperienceYears: -1,       
            Requisites: []
        );

        var handler = CreateHandler();

        // act and assert
        await Assert.ThrowsAsync<ValidationException>(() =>
             handler.Handle(invalidCmd, ct));

        _userContext.Verify(x => x.TryGetUserId(), Times.Never);
        _readRepo.Verify(x => x.CheckIfRequestExistAsync(It.IsAny<Guid>(), ct), Times.Never);
        _writeRepo.Verify(x => x.AddAsync(It.IsAny<VolunteerRequest>(), ct), Times.Never);
        _writeRepo.Verify(x => x.SaveAsync(ct), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_UserId_Not_Found_In_Context()
    {
        // arrange
        var ct = CancellationToken.None;

        var cmd = CreateValidCommand();

        var handler = CreateHandler();

        _userContext.Setup(u => u.GetUserId())
            .Throws<UserNotAuthenticatedException>();

        // act and assert
        await Assert.ThrowsAsync<UserNotAuthenticatedException>(() =>
            handler.Handle(cmd, CancellationToken.None));

        _readRepo.Verify(x => x.CheckIfRequestExistAsync(It.IsAny<Guid>(), ct), Times.Never);
        _writeRepo.Verify(x => x.AddAsync(It.IsAny<VolunteerRequest>(), ct), Times.Never);
        _writeRepo.Verify(x => x.SaveAsync(ct), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_Request_Already_Exists()
    {
        // arrange
        var ct = CancellationToken.None;
        var cmd = CreateValidCommand();
        var userId = Guid.NewGuid();

        _userContext.Setup(x => x.GetUserId())
            .Returns(userId);

        _readRepo.Setup(x => x.CheckIfRequestExistAsync(userId, ct))
            .ReturnsAsync(true); 

        var handler = CreateHandler();

        // act
        var result = await handler.Handle(cmd, ct);

        // assert
        Assert.True(result.IsFailure);

        _writeRepo.Verify(x => x.AddAsync(It.IsAny<VolunteerRequest>(), ct), Times.Never);
        _writeRepo.Verify(x => x.SaveAsync(ct), Times.Never);
    }
}


