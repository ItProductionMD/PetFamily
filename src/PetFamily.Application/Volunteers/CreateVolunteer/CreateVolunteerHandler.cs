using FluentValidation;
using PetFamily.Application.Validations;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Application.Validations.ValidationExtensions;
using static PetFamily.Application.Volunteers.VolunteerValidationExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using System.Security.AccessControl;
using PetFamily.Domain.Results;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;


//-------------------------------Handler,UseCases,Services----------------------------------------//
namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerHandler(
    IVolunteerRepository volunteerRepository,
    IValidator<CreateVolunteerCommand> validator,
    ILogger<CreateVolunteerHandler> logger)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly IValidator<CreateVolunteerCommand> _validator = validator;
    private readonly ILogger<CreateVolunteerHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(CreateVolunteerCommand command,CancellationToken cancelToken)
    {
        var validate = await _validator.ValidateAsync(command, cancelToken);
        if (validate.IsValid == false)
        {
            _logger.LogError("Fail validate volunteer command!{Errors}",validate.Errors.Select(e=>e.ErrorMessage));
            return validate.ToResultFailure<Guid>();
        }
        var getVolunteer = CreateVolunteerProccess(command);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Validate volunteer failure!{Errors}",getVolunteer.ToErrorMessages());
            return Result.Fail(getVolunteer.Errors!);
        }
        var volunteer = getVolunteer.Data!;

        var validateUniqueness = await ValidateUniqueEmailAndPhone(
            volunteer,
            _volunteerRepository,
            _logger,
            cancelToken);

        if (validateUniqueness.IsFailure)
        {
            _logger.LogWarning("Volunteer with souch email or phone is already exists!Errors:{Errors}",
                validateUniqueness.ToErrorMessages());

            return validateUniqueness;
        }
        await _volunteerRepository.Add(volunteer, cancelToken);

        _logger.LogInformation("Volunteer with id:{volunteer.Id},created sucessfull!", volunteer.Id);

        return Result.Ok(volunteer.Id);
    }
    private static Result<Volunteer> CreateVolunteerProccess(CreateVolunteerCommand command)
    {
        var fullName = FullName.Create(command.FirstName, command.LastName).Data!;

        var phone = Phone.CreateNotEmpty(command.PhoneNumber, command.PhoneRegionCode).Data!;

        var socialNetworkList = command.SocialNetworksList
            .Select(dto => SocialNetworkInfo.Create(dto.Name, dto.Url).Data!).ToList();

        var requisites = command.Requisites
            .Select(dto => RequisitesInfo.Create(dto.Name, dto.Description).Data!).ToList();

        return Volunteer.Create(
            VolunteerID.NewGuid(),
            fullName,
            command.Email,
            phone,
            command.ExperienceYears,
            command.Description,
            requisites,
            socialNetworkList);
    }
}
