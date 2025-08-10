using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.SharedApplication.Validations;
using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;
using Volunteers.Application.IRepositories;
using Volunteers.Domain;

namespace PetFamily.SharedApplication.Tests.VolunteerTests;

public class CreateVolunteerHandlerTests
{
    private readonly Mock<IVolunteerWriteRepository> _volunteerRepositoryMock;
    private readonly Mock<IVolunteerReadRepository> _volunteerReadRepositoryMock;
    private readonly Mock<ILogger<CreateVolunteerHandler>> _loggerMock;
    private readonly CreateVolunteerHandler _handler;
    private readonly CreateVolunteerFluentValidator _validator;
    private readonly Mock<IUserContext.IUserContext> _userContextMock;

    public CreateVolunteerHandlerTests()
    {
        _volunteerRepositoryMock = new Mock<IVolunteerWriteRepository>();
        _loggerMock = new Mock<ILogger<CreateVolunteerHandler>>();
        _validator = new CreateVolunteerFluentValidator();
        _volunteerReadRepositoryMock = new Mock<IVolunteerReadRepository>();
        _userContextMock = new Mock<IUserContext.IUserContext>();
        _handler = new CreateVolunteerHandler(
            _volunteerRepositoryMock.Object,
            _volunteerReadRepositoryMock.Object,
            _userContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_ThrowException_When_Command_Is_Invalid()
    {
        //ARRANGE
        var command = new CreateVolunteerCommand(
            Guid.NewGuid(),
            "invalidName!@",
            "invalidLastName!@",
            "Description",         
            0,
            "+39",
            "000000",
            []);

        //ACT and ASSERT
        await Assert.ThrowsAsync<ValidationException>(async () => 
            await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenVolunteerIsCreated()
    {
        //ARRANGE
        var command = new CreateVolunteerCommand(
            Guid.NewGuid(),
            "firstName",
            "lastName",
            "Description",           
            0,
            "+39",
            "0000000",
            []);

        var volunteer = TestDataFactory.CreateVolunteer();
        var addResult = Result.Ok(volunteer.Id);

        _volunteerRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Volunteer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(addResult);

        _userContextMock
            .Setup(x => x.GetUserId())
            .Returns(Guid.NewGuid());

        //ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        //ASSERT
        Assert.True(result.IsSuccess);
    }
}

