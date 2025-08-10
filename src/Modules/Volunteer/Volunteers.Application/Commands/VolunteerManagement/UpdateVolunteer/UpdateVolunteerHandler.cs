using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Validations;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Application.IRepositories;
using VolunteerDomain = Volunteers.Domain.Volunteer;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;

public class UpdateVolunteerHandler(
    IVolunteerWriteRepository volunteerRepository,
    ILogger<UpdateVolunteerHandler> logger,
    IVolunteerReadRepository volunteerReadRepository,
    IValidator<UpdateVolunteerCommand> validator)
    : ICommandHandler<VolunteerDomain, UpdateVolunteerCommand>
{
    private readonly IVolunteerWriteRepository _writeRepository = volunteerRepository;
    private readonly IVolunteerReadRepository _readRepository = volunteerReadRepository;
    private readonly ILogger<UpdateVolunteerHandler> _logger = logger;
    private readonly IValidator<UpdateVolunteerCommand> _validator = validator;
    public async Task<Result<VolunteerDomain>> Handle(
        UpdateVolunteerCommand cmd,
        CancellationToken ct = default)
    {
        var validateResult = await _validator.ValidateAsync(cmd, ct);
        if (validateResult.IsValid == false)
        {
            var errorResult = validateResult.ToResultFailure<VolunteerDomain>();
            _logger.LogError(
                "Fail validate volunteerRequest! Errors: {Errors}",
                errorResult.ValidationMessagesToString());

            return errorResult;
        }

        var getVolunteer = await _writeRepository.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return getVolunteer;

        var volunteer = getVolunteer.Data!;

        var fullName = FullName.Create(cmd.FirstName, cmd.LastName).Data!;


        volunteer.UpdateMainInfo(fullName, cmd.ExperienceYears, cmd.Description);

        var updateResult = await _writeRepository.Save(volunteer, ct);
        if (updateResult.IsFailure)
            return updateResult;

        _logger.LogInformation("Volunteer with id:{Id} updated successfully!", volunteer.Id);

        return Result.Ok(volunteer);
    }
}
