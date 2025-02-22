using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Validations;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
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
        UpdateVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        //--------------------------------------Validation----------------------------------------//
        var validateResult = await _validator.ValidateAsync(request, cancellationToken);
        if (validateResult.IsValid)
        {
            _logger.LogError(
                "Fail validate volunteerRequest! Errors: {Errors}",
                string.Join("; ", validateResult.Errors.Select(e=>e.ErrorMessage)));

            return validateResult.ToResultFailure<Volunteer>()!;
        }
        //-------------------------------Creating ValueObjects------------------------------------//
        var fullName = FullName.Create(request.FirstName, request.LastName).Data!;

        var phone = Phone.Create(request.PhoneNumber, request.PhoneRegionCode).Data!;

        //----------------------------Get Volunteer from database--------------------------------//
        var getVolunteer = await _volunteerRepository
            .GetByIdAsync(request.VolunteerId, cancellationToken);

        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Volunteer with id:{VolunteerId} not found!Errors:{Errors}",
                request.VolunteerId, getVolunteer.ConcateErrorMessages());

            return Result<Volunteer>.Fail(getVolunteer.Errors!);
        }
        //------------------------------------Update Volunteer------------------------------------//
        var volunteer = getVolunteer.Data!;
        var updateResultOk = volunteer.UpdateMainInfo(
            fullName,
            request.Email,
            phone,
            request.ExperienceYears,
            request.Description);

        //---------------------Verify that the email and phone number are unique------------------//
        var validateUniqueness = await ValidateUniqueEmailAndPhone(
            volunteer,
            _volunteerRepository,
            _logger,
            cancellationToken);
        if (validateUniqueness.IsFailure)
        {
            _logger.LogWarning("Fail update volunteer with souch email or phone!Errors{Errors}",
                validateUniqueness.ConcateErrorMessages());

            return Result.Fail(validateUniqueness.Errors!);
        }
        await _volunteerRepository.Save(volunteer, cancellationToken);

        _logger.LogInformation("Volunteer with id:{Id} updated successfully!", volunteer.Id);

        return Result.Ok(volunteer);
    }
}
