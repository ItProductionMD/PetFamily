using Microsoft.Extensions.Logging;
using PetFamily.Discussions.Public.Contracts;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Enums;

namespace PetFamily.VolunteerRequests.Application.Commands.TakeVolunteerRequestForReview;

public class TakeVolunteerRequestForReviewHandler(
    IVolunteerRequestWriteRepository volunteerRequestRepo,
    IDiscussionCreator discussionCreator,
    IDiscussionRemover discussionRemover,
    ILogger<TakeVolunteerRequestForReviewHandler> _logger) : ICommandHandler<TakeVolunteerRequestForReviewCommand>
{
    public async Task<UnitResult> Handle(TakeVolunteerRequestForReviewCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var adminId = cmd.AdminId;

        var getVolunteerRequest = await volunteerRequestRepo.GetByIdAsync(cmd.VolunteerRequestId, ct);
        if (getVolunteerRequest.IsFailure)
            return UnitResult.Fail(getVolunteerRequest.Error);

        var volunteerRequest = getVolunteerRequest.Data!;

        if (volunteerRequest.RequestStatus != RequestStatus.Created)
        {
            _logger.LogWarning("Volunteer request with ID {VolunteerRequestId} is already taken for review.",
                cmd.VolunteerRequestId);

            return UnitResult.Fail(Error.InternalServerError("Volunteer request is already taken for review."));
        }

        var createDiscussion = await discussionCreator.CreateDiscussion(
            volunteerRequest.Id,
            adminId,
            volunteerRequest.UserId,
            ct);

        if (createDiscussion.IsFailure)
        {
            _logger.LogWarning("Failed to create discussion for volunteer request with ID {VolunteerRequestId}. Error: {Error}",
                volunteerRequest.Id, createDiscussion.Error);

            return UnitResult.Fail(createDiscussion.Error);
        }
        var discussionId = createDiscussion.Data!;

        volunteerRequest.TakeToReview(adminId, discussionId);
        try
        {
            await volunteerRequestRepo.SaveAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save volunteer request with ID {VolunteerRequestId}. Error: {Message}",
                volunteerRequest.Id, ex.Message);

            await discussionRemover.RemoveDisscusion(discussionId);

            return UnitResult.Fail(Error.InternalServerError("Failed to save volunteer request after taking it for review."));
        }
        _logger.LogInformation("Volunteer request with ID {VolunteerRequestId} has been taken for review by admin ID {AdminId}.",
            volunteerRequest.Id, adminId);

        return UnitResult.Ok();
    }
}
