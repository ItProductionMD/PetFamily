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

    public CreateVolunteerHandlerTests()
    {
        _volunteerRepositoryMock = new Mock<IVolunteerWriteRepository>();
        _loggerMock = new Mock<ILogger<CreateVolunteerHandler>>();
        _validator = new CreateVolunteerFluentValidator();
        _volunteerReadRepositoryMock = new Mock<IVolunteerReadRepository>();
        _handler = new CreateVolunteerHandler(
            _volunteerRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_ThrowException_When_Command_Is_Invalid()
    {
        //ARRANGE
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateVolunteerCommand(
            adminId,
            userId,
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
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateVolunteerCommand(
            adminId,
            userId,
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
            .Setup(x => x.AddAndSaveAsync(It.IsAny<Volunteer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(addResult);

        //ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        //ASSERT
        Assert.True(result.IsSuccess);
    }
}

