using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.Application.Validations;
using PetFamily.SharedApplication.IUserContext;
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
    private readonly Mock<IUserContext> _userContextMock;

    public CreateVolunteerHandlerTests()
    {
        _volunteerRepositoryMock = new Mock<IVolunteerWriteRepository>();
        _validatorMock = new Mock<IValidator<CreateVolunteerCommand>>();
        _loggerMock = new Mock<ILogger<CreateVolunteerHandler>>();
        _validator = new CreateVolunteerCommandValidator();
        _volunteerReadRepositoryMock = new Mock<IVolunteerReadRepository>();
        _userContextMock = new Mock<IUserContext>();
        _handler = new CreateVolunteerHandler(
            _volunteerRepositoryMock.Object,
            _volunteerReadRepositoryMock.Object,
            _validatorMock.Object,
            _userContextMock.Object,
            _loggerMock.Object);
    }

    [Theory]
    [InlineData("firstName", "firstName?!", "lastName", 0)]
    [InlineData("lastName", "firstName", "lastName?", 0)]
    [InlineData("experienceYears", "firstName", "lastName", 345)]
    public void ValidateCreateVolunteerCommand_WithValidationError_ShouldReturnFailure(
        string invalidFieldName,
        string firstName,
        string lastName,
        int experienceYears)
    {
        // ARRANGE
        var command = new CreateVolunteerCommand(
            firstName,
            lastName,        
            "Description",           
            experienceYears,
            []);

        //ACT
        var result = _validator.Validate(command).ToResultFailure();

        // ASSERT
        Assert.True(result.IsFailure);
        Assert.Contains(invalidFieldName.ToLower(), result.ValidationMessagesToString().ToLower());
    }

    [Theory]
    [InlineData("firstName", 1000, 5)]
    [InlineData("lastName", 5, 1000)]
    public void ValidateCreateVolunteerCommand_WithValidationErrorTextLength_ShouldReturnFailure(
        string invalidFieldName,
        int firstNameSize,
        int lastNameSize)
    {
        // ARRANGE
        var firstName = new string('a', firstNameSize);
        var lastName = new string('b', lastNameSize);

        var command = new CreateVolunteerCommand(
            firstName,
            lastName,     
            "Description",
            0,
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
            "Description",         
            0,
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
            "Description",           
            0,
            []);

        var volunteer = TestDataFactory.CreateVolunteer();
        var addResult = Result.Ok(volunteer.Id);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());


        _volunteerRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Volunteer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(addResult);

        _userContextMock
            .Setup(x => x.GetUserId())
            .Returns(Result.Ok(Guid.NewGuid()));

        _userContextMock
            .Setup(x => x.Phone)
            .Returns("+373-69999999");


        //ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        //ASSERT
        Assert.True(result.IsSuccess);
    }
}

