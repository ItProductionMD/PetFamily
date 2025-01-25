using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Validations;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using static PetFamily.Application.Volunteers.VolunteerValidationExtensions;

namespace PetFamily.Application.Volunteers.UpdateVolunteer;

public class UpdateVolunteerHandler(
    IVolunteerRepository volunteerRepository,
    ILogger<UpdateVolunteerHandler> logger,
    IValidator<UpdateVolunteerRequest> validator)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<UpdateVolunteerHandler> _logger = logger;
    private readonly IValidator<UpdateVolunteerRequest> _validator = validator;
    public async Task<Result<Volunteer>> Handle(
        UpdateVolunteerRequest volunteerRequest,
        CancellationToken cancellationToken = default)
    {
        //--------------------------------------Validation----------------------------------------//
        var validateResult = await _validator.ValidateAsync(volunteerRequest, cancellationToken);
        if (validateResult.IsValid == false)
        {
            _logger.LogError(
                "Validate volunteerRequest failure!{validateResult.Errors}",
                validateResult.Errors);

            return validateResult.ToResultFailure<Volunteer>();
        }
        //-------------------------------Creating ValueObjects------------------------------------//
        var fullName = FullName.Create(volunteerRequest.FirstName, volunteerRequest.LastName).Data;

        var phone = Phone.Create(volunteerRequest.PhoneNumber, volunteerRequest.PhoneRegionCode).Data;

        //----------------------------Get Volunteer ffrom database--------------------------------//
        var getVolunteer = await _volunteerRepository
            .GetById(volunteerRequest.VolunteerId, cancellationToken);

        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Volunteer not found!{getVolunteer.Errors}", getVolunteer.Errors);
            return Result<Volunteer>.Failure(getVolunteer.Errors!);
        }
        //------------------------------------Update Volunteer------------------------------------//
        var volunteer = getVolunteer.Data;
        var updateResultOk = volunteer.UpdateMainInfo(
            fullName,
            volunteerRequest.Email,
            phone,
            volunteerRequest.ExperienceYears,
            volunteerRequest.Description);

        //---------------------Verify that the email and phone number are unique------------------//
        var validateUniqueness = await ValidateUniqueEmailAndPhone(
            volunteer,
            _volunteerRepository,
            _logger,
            cancellationToken);

        if (validateUniqueness.IsFailure)
            return Result<Volunteer>.Failure(validateUniqueness.Errors!);

        //--------------------------Save updated Volunteer in database----------------------------//
        var saveChanges = await _volunteerRepository.Save(volunteer, cancellationToken);
        if (saveChanges.IsFailure)
        {
            _logger.LogError("Volunteer update error!{getVolunteer.Errors}",saveChanges.Errors);
            return Result<Volunteer>.Failure(saveChanges.Errors!);
        }
        //----------------------------------------------------------------------------------------//
        _logger.LogInformation("Volunteer with id:{volunteer.Id} updated successfully!", volunteer.Id);

        return Result<Volunteer>.Success(volunteer);
    }
}
