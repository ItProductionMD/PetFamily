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
    private readonly Mock<IVolunteerRepository> _volunteerRepositoryMock;
    private readonly Mock<IValidator<CreateVolunteerCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateVolunteerHandler>> _loggerMock;
    private readonly CreateVolunteerHandler _handler;
    private readonly CreateVolunteerCommandValidator _validator;

    public CreateVolunteerHandlerTests()
    {
        _volunteerRepositoryMock = new Mock<IVolunteerRepository>();
        _validatorMock = new Mock<IValidator<CreateVolunteerCommand>>();
        _loggerMock = new Mock<ILogger<CreateVolunteerHandler>>();
        _validator = new CreateVolunteerCommandValidator();
        _handler = new CreateVolunteerHandler(
            _volunteerRepositoryMock.Object,
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
        string invalidFieldName,
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
        Assert.Single(unitResult.Errors);

    }
    [Theory]
    [InlineData("firstName", 1000, 5, 5)]
    [InlineData("lastName", 5, 1000, 5)]
    [InlineData("email", 5, 5, 1000)]
    public void ValidateCreateVolunteerCommand_WithValidationErrorTextLength_ShouldReturnFailure(
        string invalidFieldName,
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
        Assert.Single(unitResult.Errors);

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
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fail validate volunteer command")),
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

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _volunteerRepositoryMock.Setup(x => x.Add(It.IsAny<Volunteer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _volunteerRepositoryMock
            .Setup(x=>x.GetByEmailOrPhone(It.IsAny<string>(),It.IsAny<Phone>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(Error.NotFound("Volunteer")));
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _loggerMock.Verify(x => 
        x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) =>
            v.ToString().Contains("Volunteer with id:")), 
            null, 
            It.IsAny<Func<It.IsAnyType, Exception?,
            string>>()), Times.Once);
    }
}

