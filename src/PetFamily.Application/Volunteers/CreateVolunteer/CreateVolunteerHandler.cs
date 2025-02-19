using FluentValidation;
using PetFamily.Application.Validations;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using PetFamily.Domain.VolunteerAggregates.ValueObjects;
using static PetFamily.Domain.Shared.ValueObjects.ValueObjectFactory;
using static PetFamily.Application.Validations.ValidationExtensions;
using static PetFamily.Application.Volunteers.VolunteerValidationExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using System.Security.AccessControl;
using PetFamily.Domain.Results;


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

    public async Task<Result<Guid>> Handle(
        CreateVolunteerCommand volunteerRequest,
        CancellationToken cancellationToken = default)
    {
        //--------------------------------------Fluent Validator----------------------------------//
        var fluentValidationResult = await _validator.ValidateAsync(
            volunteerRequest,
            cancellationToken);
        if (fluentValidationResult.IsValid == false)
        {
            _logger.LogError("Fail validate volunteerRequest!{Errors}",
                           fluentValidationResult.Errors.Select(e=>e.ErrorMessage));

            return fluentValidationResult.ToResultFailure<Guid>();
        }
        //-------------------------------Creating ValueObjects------------------------------------//
        var fullName = FullName.Create(volunteerRequest.FirstName, volunteerRequest.LastName).Data!;

        var phone = Phone.Create(volunteerRequest.PhoneNumber, volunteerRequest.PhoneRegionCode).Data!;

        var socialNetworkList = volunteerRequest.SocialNetworksList
            .Select(dto => SocialNetworkInfo.Create(dto.Name, dto.Url).Data!).ToList();

        var requisites = volunteerRequest.Requisites
           .Select(dto => RequisitesInfo.Create(dto.Name, dto.Description).Data!).ToList();

        var volunteerCreateResult = Volunteer.Create(
            VolunteerID.NewGuid(),
            fullName,
            volunteerRequest.Email,
            phone,
            volunteerRequest.ExperienceYears,
            volunteerRequest.Description,
            requisites,
            socialNetworkList);

        if (volunteerCreateResult.IsFailure)
        {
            _logger.LogError("Validate volunteer entity failure!{ volunteerCreateResult.Errors}",
                volunteerCreateResult.ConcateErrorMessages());

            return Result.Fail(volunteerCreateResult.Errors!);
        }
        var volunteer = volunteerCreateResult.Data!;
        //---------------------Verify that the email and phone number are unique------------------//
        var validateUniqueness = await ValidateUniqueEmailAndPhone(
            volunteer,
            _volunteerRepository,
            _logger,
            cancellationToken);

        if (validateUniqueness.IsFailure)
        {
            _logger.LogWarning("Volunteer with souch email or phone is already exists!Errors:{Errors}",
                validateUniqueness.ConcateErrorMessages());

            return validateUniqueness;
        }
        //---------------------------------Add Volunteer to repository----------------------------//
        await _volunteerRepository.Add(volunteer, cancellationToken);

        _logger.LogInformation("Volunteer with id:{volunteer.Id},created sucessfull!", volunteer.Id);

        return Result.Ok(volunteer.Id);
    }
}
