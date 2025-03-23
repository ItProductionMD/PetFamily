using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Application.Validations;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.Commands.VolunteerManagment.UpdateVolunteer;

public class UpdateVolunteerHandler(
    IVolunteerWriteRepository volunteerRepository,
    ILogger<UpdateVolunteerHandler> logger,
    IVolunteerReadRepository volunteerReadRepository,
    IValidator<UpdateVolunteerCommand> validator):ICommandHandler<Volunteer,UpdateVolunteerCommand>
{
    private readonly IVolunteerWriteRepository _volunteerRepository = volunteerRepository;
    private readonly IVolunteerReadRepository _volunteerReadRepository = volunteerReadRepository;
    private readonly ILogger<UpdateVolunteerHandler> _logger = logger;
    private readonly IValidator<UpdateVolunteerCommand> _validator = validator;
    public async Task<Result<Volunteer>> Handle(
        UpdateVolunteerCommand command,
        CancellationToken cancelToken = default)
    {
        var validateResult = await _validator.ValidateAsync(command, cancelToken);
        if (validateResult.IsValid == false)
        {
            var errorResult = validateResult.ToResultFailure<Volunteer>();
            _logger.LogError(
                "Fail validate volunteerRequest! Errors: {Errors}", 
                errorResult.ValidationMessagesToString());

            return errorResult;
        }

        var checkUniqueness = await _volunteerReadRepository.CheckUniqueFields(
            command.VolunteerId,
            command.PhoneRegionCode,
            command.PhoneNumber,
            command.Email,
            cancelToken);
        if (checkUniqueness.IsFailure)
            return checkUniqueness;

        var getVolunteer = await _volunteerRepository.GetByIdAsync(command.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return getVolunteer;

        var volunteer = getVolunteer.Data!;

        var fullName = FullName.Create(command.FirstName, command.LastName).Data!;

        var phone = Phone.CreateNotEmpty(command.PhoneNumber, command.PhoneRegionCode).Data!;
    
        volunteer.UpdateMainInfo(fullName, command.Email, phone, command.ExperienceYears, command.Description);

        var updateResult = await _volunteerRepository.Save(volunteer, cancelToken);
        if (updateResult.IsFailure)
            return updateResult;

        _logger.LogInformation("Volunteer with id:{Id} updated successfully!", volunteer.Id);

        return Result.Ok(volunteer);
    }
}
