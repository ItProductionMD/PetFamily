using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Commands.RejectVolunteerRequest;

public class RejectVolunteerRequestHandler(
    IVolunteerRequestWriteRepository requestRepo,
    ILogger<RejectVolunteerRequestHandler> logger) : ICommandHandler<RejectVolunteerRequestCommand>
{
    public async Task<UnitResult> Handle(RejectVolunteerRequestCommand cmd, CancellationToken ct)
    {
        //TODO ADD VALIDATION

        var adminId = cmd.AdminId; ;

        var getRequest = await requestRepo.GetByIdAsync(cmd.VolunteerRequestId, ct);
        if (getRequest.IsFailure)
            return UnitResult.Fail(getRequest.Error);

        var request = getRequest.Data!;

        var rejectResult = request.Reject(adminId, cmd.Comment);
        if (rejectResult.IsFailure)
        {
            logger.LogWarning("Reject failed for request ID {Id}: {Error}",
                cmd.VolunteerRequestId, rejectResult.Error);
            return UnitResult.Fail(rejectResult.Error);
        }

        await requestRepo.SaveAsync(ct);

        logger.LogInformation("Volunteer request with ID {Id} rejected by admin {AdminId}",
            cmd.VolunteerRequestId, adminId);

        return UnitResult.Ok();
    }
}
