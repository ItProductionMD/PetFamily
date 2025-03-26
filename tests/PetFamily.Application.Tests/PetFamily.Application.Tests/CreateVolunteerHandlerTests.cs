using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.Application.Commands.VolunteerManagment.CreateVolunteer;
using PetFamily.Application.IRepositories;
using PetFamily.Application.Validations;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using Xunit;

namespace PetFamily.Application.Tests;

public class CreateVolunteerHandlerTests
{
    private readonly Mock<IVolunteerWriteRepository> _volunteerRepositoryMock;
    private readonly Mock<IVolunteerReadRepository> _volunteerReadRepositoryMock;

    private readonly Mock<IValidator<CreateVolunteerCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateVolunteerHandler>> _loggerMock;
    private readonly CreateVolunteerHandler _handler;
    private readonly CreateVolunteerCommandValidator _validator;

    public CreateVolunteerHandlerTests()
    {
        _volunteerRepositoryMock = new Mock<IVolunteerWriteRepository>();
        _validatorMock = new Mock<IValidator<CreateVolunteerCommand>>();
        _loggerMock = new Mock<ILogger<CreateVolunteerHandler>>();
        _validator = new CreateVolunteerCommandValidator();
        _volunteerReadRepositoryMock = new Mock<IVolunteerReadRepository>();
        _handler = new CreateVolunteerHandler(
            _volunteerRepositoryMock.Object,
            _volunteerReadRepositoryMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Theory]
    [InlineData("firstName", "firstName?!", "lastName", "rubon@gmail.com", "6767678876", "+373", 0)]
    [InlineData("lastName", "firstName", "lastName?", "rubon@gmail.com", "6767678876", "+373", 0)]
    [InlineData("email", "firstName", "lastName", "rubongmail.com", "6767678876", "+373", 0)]
    [InlineData("phoneNumber", "firstName", "lastName", "rubon@gmail.com", "67676788A6", "+373", 0)]
    [InlineData("phoneRegion", "firstName", "lastName", "rubon@gmail.com", "6767678876", "+37D3", 0)]
    [InlineData("expirienceYears", "firstName", "lastName", "rubon@gmail.com", "6767678876", "+373", 345)]
    public void ValidateCreateVolunteerCommand_WithValidationError_ShouldReturnFailure(
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        string invalidFieldName,
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string phoneRegionCode,
        int expirienceYears)
    {
        // Arrange
        var command = new CreateVolunteerCommand(
            firstName,
            lastName,
            email,
            "Description",
            phoneNumber,
            phoneRegionCode,
            expirienceYears,
            [],
            []);
        //ACT
        var validationResult = _validator.Validate(command);
        var unitResult = validationResult.ToResultFailure();
        // Assert
        Assert.False(validationResult.IsValid);

    }
    [Theory]
    [InlineData("firstName", 1000, 5, 5)]
    [InlineData("lastName", 5, 1000, 5)]
    [InlineData("email", 5, 5, 1000)]
    public void ValidateCreateVolunteerCommand_WithValidationErrorTextLength_ShouldReturnFailure(
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        string invalidFieldName,
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
        int firstNameSize,
        int lastNameSize,
        int emailSize)
    {
        var firstName = new string('a', firstNameSize);
        var lastName = new string('b', lastNameSize);
        var email = new string('a', emailSize) + "@gmail.com";
        // Arrange
        var command = new CreateVolunteerCommand(
            firstName,
            lastName,
            email,
            "Description",
            "765367992",
            "+373",
            0,
            [],
            []);
        //ACT
        var validationResult = _validator.Validate(command);
        var unitResult = validationResult.ToResultFailure();
        // Assert
        Assert.False(validationResult.IsValid);

    }
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenValidateVolunteerCommandFails()
    {
        // Arrange
        var command = new CreateVolunteerCommand(
            "invalidName!@",
            "lastName",
            "email@gmail.com",
            "Description",
            "765367992",
            "+373",
            0,
            [],
            []);

        var validationFailure = new ValidationFailure("", "");
        validationFailure.ErrorCode = "";
        var validationResult = new ValidationResult([validationFailure]);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        _loggerMock.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fail validate volunteer command")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenVolunteerIsCreated()
    {
        // Arrange
        var command = new CreateVolunteerCommand(
            "firstName",
            "lastName",
            "email@gmail.com",
            "Description",
            "765367992",
            "+373",
            0,
            [],
            []);

        var volunteer = TestDataFactory.CreateVolunteer();
        var addResult = Result.Ok(volunteer.Id);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _volunteerReadRepositoryMock
            .Setup(v => v.CheckUniqueFields(
                Guid.Empty,
                command.PhoneRegionCode,
                command.PhoneNumber,
                command.Email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Ok());

        _volunteerRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Volunteer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(addResult);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _loggerMock.Verify(x =>
        x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) =>
            v.ToString()!.Contains("Volunteer with id:")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?,
            string>>()), Times.Once);
    }
}

