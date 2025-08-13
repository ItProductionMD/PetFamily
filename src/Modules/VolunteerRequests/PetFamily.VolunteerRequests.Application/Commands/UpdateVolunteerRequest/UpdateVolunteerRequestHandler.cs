using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Commands.UpdateVolunteerRequest;

public class UpdateVolunteerRequestHandler(
    IVolunteerRequestWriteRepository requestRepo,
    ILogger<UpdateVolunteerRequestHandler> logger) : ICommandHandler<UpdateVolunteerRequestCommand>
{
    public async Task<UnitResult> Handle(UpdateVolunteerRequestCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var userId = cmd.UserId;

        var getRequest = await requestRepo.GetByIdAsync(cmd.VolunteerRequestId, ct);
        if (getRequest.IsFailure)
            return UnitResult.Fail(getRequest.Error);

        var request = getRequest.Data!;

        if (request.UserId != userId)
        {
            logger.LogWarning("User {UserId} tried to update someone else's volunteer request " +
                "{RequestId}", userId, request.Id);
            return UnitResult.Fail(Error.Forbidden("You are not the owner of this volunteer request."));
        }

        var updateResult = request.UpdateRequestDetails(
            cmd.FirstName,
            cmd.LastName,
            cmd.ExperienceYears,
            cmd.DocumentName,
            cmd.Description
        );

        if (updateResult.IsFailure)
        {
            logger.LogWarning("Failed to update volunteer request {RequestId}: {Error}",
                cmd.VolunteerRequestId, updateResult.Error);
            return UnitResult.Fail(updateResult.Error);
        }

        await requestRepo.SaveAsync(ct);

        logger.LogInformation("Volunteer request {RequestId} successfully updated", request.Id);

        return UnitResult.Ok();
    }
}

