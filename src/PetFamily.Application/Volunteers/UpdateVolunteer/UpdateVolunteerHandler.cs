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
    IValidator<UpdateVolunteerCommand> validator)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<UpdateVolunteerHandler> _logger = logger;
    private readonly IValidator<UpdateVolunteerCommand> _validator = validator;
    public async Task<Result<Volunteer>> Handle(
        UpdateVolunteerCommand command,
        CancellationToken cancelToken = default)
    {
        //--------------------------------------Validation----------------------------------------//
        var validateResult = await _validator.ValidateAsync(command, cancelToken);
        if (validateResult.IsValid)
        {
            _logger.LogError(
                "Fail validate volunteerRequest! Errors: {Errors}",
                string.Join("; ", validateResult.Errors.Select(e => e.ErrorMessage)));

            return validateResult.ToResultFailure<Volunteer>()!;
        }
        //-------------------------------Creating ValueObjects------------------------------------//
        var fullName = FullName.Create(command.FirstName, command.LastName).Data!;

        var phone = Phone.Create(command.PhoneNumber, command.PhoneRegionCode).Data!;

        //----------------------------Get Volunteer from database--------------------------------//
        var volunteer = await _volunteerRepository.GetByIdAsync(command.VolunteerId, cancelToken);

        volunteer.UpdateMainInfo(fullName, command.Email, phone, command.ExperienceYears, command.Description);

        //---------------------Verify that the email and phone number are unique------------------//
        var validateUniqueness = await ValidateUniqueEmailAndPhone(
            volunteer,
            _volunteerRepository,
            _logger,
            cancelToken);
        if (validateUniqueness.IsFailure)
        {
            _logger.LogWarning("Fail update volunteer with souch email or phone!Errors{Errors}",
                validateUniqueness.ToErrorMessages());

            return Result.Fail(validateUniqueness.Errors!);
        }
        await _volunteerRepository.Save(volunteer, cancelToken);

        _logger.LogInformation("Volunteer with id:{Id} updated successfully!", volunteer.Id);

        return Result.Ok(volunteer);
    }
}
