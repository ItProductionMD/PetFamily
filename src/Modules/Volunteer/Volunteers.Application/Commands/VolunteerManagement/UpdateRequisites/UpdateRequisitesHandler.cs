using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateRequisites;

public class UpdateRequisitesHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    ILogger<UpdateRequisitesHandler> logger) : ICommandHandler<UpdateRequisitesCommand>
{
    public async Task<UnitResult> Handle(
        UpdateRequisitesCommand cmd,
        CancellationToken ct = default)
    {
        cmd.Validate();

        var getVolunteer = await volunteerWriteRepo.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;
        if (volunteer.UserId.Value != cmd.UserId)
        {
            logger.LogError("Access for update volunteer requisites with volunteer id:{VolunteerId}" +
                "  forbidden for user with id: {UserId}", cmd.VolunteerId, cmd.UserId);
            return UnitResult.Fail(Error.Forbidden($"Access for update volunteer requisites with " +
                $"volunteer id:{cmd.VolunteerId} forbidden for user with id: {cmd.UserId}"));
        }
        var requisites = cmd.RequisitesDtos.Select(c =>
            RequisitesInfo.Create(c.Name, c.Description).Data!);

        volunteer.UpdateRequisites(requisites);

        await volunteerWriteRepo.SaveAsync(volunteer, ct);

        logger.LogInformation("Update requisites for volunteer with id:{Id} successful!",
            cmd.VolunteerId);

        return UnitResult.Ok();
    }
}
