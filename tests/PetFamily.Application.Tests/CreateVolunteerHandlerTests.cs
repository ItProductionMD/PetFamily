using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.Application.Validations;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;
using Volunteers.Application.IRepositories;
using Volunteers.Domain;

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
    [InlineData("phone Number", "firstName", "lastName", "rubon@gmail.com", "67676788A6", "+373", 0)]
    [InlineData("phone regionCode", "firstName", "lastName", "rubon@gmail.com", "6767678876", "+37D3", 0)]
    [InlineData("experienceYears", "firstName", "lastName", "rubon@gmail.com", "6767678876", "+373", 345)]
    public void ValidateCreateVolunteerCommand_WithValidationError_ShouldReturnFailure(
        string invalidFieldName,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string phoneRegionCode,
        int expirienceYears)
    {
        // ARRANGE
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
        var result = _validator.Validate(command).ToResultFailure();

        // ASSERT
        Assert.True(result.IsFailure);
        Assert.Contains(invalidFieldName.ToLower(), result.ValidationMessagesToString().ToLower());
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
        // ARRANGE
        var firstName = new string('a', firstNameSize);
        var lastName = new string('b', lastNameSize);
        var email = new string('a', emailSize) + "@gmail.com";

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
        var result = _validator.Validate(command).ToResultFailure();

        // ASSERT
        Assert.True(result.IsFailure);
        Assert.Contains(invalidFieldName.ToLower(), result.ValidationMessagesToString().ToLower());
        Assert.Contains("length", result.ValidationMessagesToString().ToLower());

    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenValidateVolunteerCommandFails()
    {
        //ARRANGE
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

        var validationResult = new ValidationResult([new ValidationFailure()]);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        //ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        //ASSERT
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenVolunteerIsCreated()
    {
        //ARRANGE
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

        //ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        //ASSERT
        Assert.True(result.IsSuccess);
    }
}

