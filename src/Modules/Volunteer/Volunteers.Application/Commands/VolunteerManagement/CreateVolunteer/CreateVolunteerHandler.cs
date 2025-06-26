using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Application.Validations;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using System.Net.WebSockets;
using Volunteers.Application.IRepositories;
using Volunteers.Domain.ValueObjects;


//-------------------------------Handler,UseCases,Services----------------------------------------//
namespace Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

public class CreateVolunteerHandler(
    IVolunteerWriteRepository volunteerWriteRepository,
    IVolunteerReadRepository volunteerReadRepository,
    IValidator<CreateVolunteerCommand> validator,
    IUserContext userContext,
    ILogger<CreateVolunteerHandler> logger) : ICommandHandler<Guid, CreateVolunteerCommand>
{
    private readonly IVolunteerWriteRepository _volunteerWriteRepository = volunteerWriteRepository;
    private readonly IVolunteerReadRepository _volunteerReadRepository = volunteerReadRepository;
    private readonly IValidator<CreateVolunteerCommand> _validator = validator;
    private readonly ILogger<CreateVolunteerHandler> _logger = logger;
    private readonly IUserContext _userContext = userContext;

    public async Task<Result<Guid>> Handle(CreateVolunteerCommand cmd, CancellationToken ct)
    {
        var fluentValidationResult = await _validator.ValidateAsync(cmd, ct);
        if (fluentValidationResult.IsValid == false)
        {
            var errorResult = fluentValidationResult.ToResultFailure<Guid>();
            _logger.LogError("Fail validate volunteer command!{Errors}",
                errorResult.ValidationMessagesToString());

            return errorResult;
        }
        var getUserId = _userContext.GetUserId();
        if (getUserId.IsFailure)
            return Result.Fail(getUserId.Error);

        var formattedPhone = _userContext.Phone;
        if (string.IsNullOrWhiteSpace(formattedPhone))
        {
            _logger.LogError("User phone is not set! UserId:{UserId}", getUserId.Data);
            return Result.Fail(Error.NotFound("Phone in user context"));
        }
        var phoneResult = Phone.FromStringFormattedPhone(formattedPhone);
        if(phoneResult.IsFailure)
        {
            _logger.LogError("User phone in user context is not valid! UserId:{UserId},Phone:{Phone}",
                getUserId.Data, formattedPhone);
            return Result.Fail(phoneResult.Error);
        }
        var phone = phoneResult.Data!;

        var userId = getUserId.Data!;

        var volunteer = CreateVolunteerProcess(userId,phone, cmd);

        var addResult = await _volunteerWriteRepository.AddAsync(volunteer, ct);
        if (addResult.IsFailure)
            return addResult;

        _logger.LogInformation("Volunteer with id:{Id},created successful!", volunteer.Id);

        return Result.Ok(volunteer.Id);
    }

    private static Volunteers.Domain.Volunteer CreateVolunteerProcess(
        Guid userId,
        Phone phone,
        CreateVolunteerCommand command)
    {
        var fullName = FullName.Create(command.FirstName, command.LastName).Data!;

        var requisites = command.Requisites
            .Select(dto => RequisitesInfo.Create(dto.Name, dto.Description).Data!).ToList();

        return Volunteers.Domain.Volunteer.Create(
            VolunteerID.NewGuid(),
            UserId.Create(userId).Data!,
            fullName,
            command.ExperienceYears,
            command.Description,
            phone,
            requisites).Data!;
    }
}
