using Microsoft.Extensions.Logging;
using PetFamily.Discussions.Public.Contracts;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Commands.SendVolunteerRequestToRevision;

public class SendRequestToRevisionHandler(
    IVolunteerRequestWriteRepository volunteerRequestWriteRepo,
    IDiscussionMessageSender messageSender,
    ILogger<SendRequestToRevisionHandler> logger) : ICommandHandler<SendRequestToRevisionCommand>
{
    public async Task<UnitResult> Handle(SendRequestToRevisionCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var adminId = cmd.AdminId;

        var getVolunteerRequest = await volunteerRequestWriteRepo.GetByIdAsync(cmd.VolunteerRequestId, ct);
        if (getVolunteerRequest.IsFailure)
            return UnitResult.Fail(getVolunteerRequest.Error);

        var request = getVolunteerRequest.Data!;

        var sendToRevisionResult = request.SendBackToRevision(adminId);
        if (sendToRevisionResult.IsFailure)
        {
            logger.LogError("Failed to send request to revision: {Error}", sendToRevisionResult.Error);
            return UnitResult.Fail(sendToRevisionResult.Error);
        }

        await volunteerRequestWriteRepo.SaveAsync(ct);

        // Try send message to discussion
        try
        {
            var sendMessageResult = await messageSender.Send(
                cmd.VolunteerRequestId,
                adminId,
                cmd.Comment,
                ct);

            if (sendMessageResult.IsFailure)
                throw new Exception(sendMessageResult.Error.Message);
        }
        catch (Exception ex)
        {
            var rollbackResult = request.CancelSendBackToRevision(adminId);
            if (rollbackResult.IsSuccess)
                await volunteerRequestWriteRepo.SaveAsync(ct);

            logger.LogError(ex, "Failed to send message to discussion: {Message}", ex.Message);
            return UnitResult.Fail(Error.InternalServerError("Send request to revision failed!" +
                " Cause: Discussion service is unavailable"));
        }
        logger.LogInformation("Send request to revision for VolunteerRequestId {Id} successful!",
            cmd.VolunteerRequestId);

        return UnitResult.Ok();
    }
}

