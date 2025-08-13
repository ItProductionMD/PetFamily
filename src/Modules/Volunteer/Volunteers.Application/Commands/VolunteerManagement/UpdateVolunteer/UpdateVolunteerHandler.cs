using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Validations;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Application.IRepositories;
using VolunteerDomain = Volunteers.Domain.Volunteer;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;

public class UpdateVolunteerHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    ILogger<UpdateVolunteerHandler> logger,
    IVolunteerReadRepository volunteerReadRepo) : ICommandHandler<VolunteerDomain, UpdateVolunteerCommand>
{
    private readonly IVolunteerWriteRepository _volunteerWriteRepo = volunteerWriteRepo;
    private readonly IVolunteerReadRepository _volunteerReadRepo = volunteerReadRepo;
    private readonly ILogger<UpdateVolunteerHandler> _logger = logger;

    public async Task<Result<VolunteerDomain>> Handle(
        UpdateVolunteerCommand cmd,
        CancellationToken ct = default)
    {
        cmd.Validate();

        var getVolunteer = await _volunteerWriteRepo.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return getVolunteer;

        var volunteer = getVolunteer.Data!;

        if(volunteer.UserId.Value != cmd.UserId)
        {
            _logger.LogError("Access  for update volunteer with Id: {VolunteerId } - forbidden for " +
                "user with id: {UserId}",cmd.VolunteerId, cmd.UserId);
            return Result.Fail(Error.Forbidden($"Access forbidden for user with id: {cmd.UserId}"));
        }

        var fullName = FullName.Create(cmd.FirstName, cmd.LastName).Data!;

        volunteer.UpdateMainInfo(fullName, cmd.ExperienceYears, cmd.Description);

        var updateResult = await _volunteerWriteRepo.SaveAsync(volunteer, ct);
        if (updateResult.IsFailure)
            return updateResult;

        _logger.LogInformation("Volunteer with id:{Id} updated successfully!", volunteer.Id);

        return Result.Ok(volunteer);
    }
}
