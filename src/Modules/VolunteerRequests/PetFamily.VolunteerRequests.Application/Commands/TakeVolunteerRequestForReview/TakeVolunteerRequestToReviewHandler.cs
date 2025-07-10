using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Discussions.Public.Contracts;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Enums;

namespace PetFamily.VolunteerRequests.Application.Commands.TakeVolunteerRequestForReview;

public class TakeVolunteerRequestToReviewHandler(
    IVolunteerRequestWriteRepository requestRepository,
    IDiscussionCreator discussionCreator,
    IDiscussionRemover discussionRemover,
    IUserContext userContext,
    ILogger<TakeVolunteerRequestToReviewHandler> logger) : ICommandHandler<TakeVolunteerRequestForReviewCommand>
{
    private readonly IVolunteerRequestWriteRepository _requestRepository = requestRepository;
    private readonly ILogger<TakeVolunteerRequestToReviewHandler> _logger = logger;
    private readonly IUserContext _userContext = userContext;
    private readonly IDiscussionRemover _discussionRemover = discussionRemover;
    private readonly IDiscussionCreator _discussionCreator = discussionCreator;

    public async Task<UnitResult> Handle(TakeVolunteerRequestForReviewCommand cmd, CancellationToken ct)
    {
        var validateCommand = TakeVolunteerRequestForReviewValidator.Validate(cmd);
        if (validateCommand.IsFailure)
        {
            _logger.LogWarning("Validation failed for TakeVolunteerRequestForReviewCommand. Error: {Error}",
                validateCommand.Error);
            return UnitResult.Fail(validateCommand.Error);
        }

        var getVolunteerRequest = await _requestRepository.GetByIdAsync(cmd.VolunteerRequestId, ct);
        if (getVolunteerRequest.IsFailure)
        {
            _logger.LogWarning("Failed to retrieve volunteer request with ID {VolunteerRequestId}. Error: {Error}",
                cmd.VolunteerRequestId, getVolunteerRequest.Error);

            return UnitResult.Fail(getVolunteerRequest.Error);
        }

        var volunteerRequest = getVolunteerRequest.Data!;
        if (volunteerRequest.RequestStatus != RequestStatus.Created)
        {
            _logger.LogWarning("Volunteer request with ID {VolunteerRequestId} is already taken for review.",
                cmd.VolunteerRequestId);

            return UnitResult.Fail(Error.InternalServerError("Volunteer request is already taken for review."));
        }
        var getAdminId = _userContext.GetUserId();
        if (getAdminId.IsFailure)
            return UnitResult.Fail(getAdminId.Error);

        var adminId = getAdminId.Data!;

        var createDiscussion = await _discussionCreator.CreateDiscussion(
            volunteerRequest.Id,
            [volunteerRequest.UserId, adminId],
            ct);

        if (createDiscussion.IsFailure)
        {
            _logger.LogWarning("Failed to create discussion for volunteer request with ID {VolunteerRequestId}. Error: {Error}",
                volunteerRequest.Id, createDiscussion.Error);

            return UnitResult.Fail(createDiscussion.Error);
        }
        var discussionId = createDiscussion.Data!;

        volunteerRequest.TakeToReview(adminId, discussionId);

        var saveResult = await _requestRepository.SaveAsync(ct);

        if (saveResult.IsFailure)
        {
            _logger.LogWarning("Failed to save volunteer request with ID {VolunteerRequestId}. Error: {Error}",
                volunteerRequest.Id, saveResult.Error);

            await _discussionRemover.RemoveDisscusion(discussionId);

            return UnitResult.Fail(saveResult.Error);
        }
        _logger.LogInformation("Volunteer request with ID {VolunteerRequestId} has been taken for review by admin ID {AdminId}.",
            volunteerRequest.Id, adminId);

        return UnitResult.Ok();
    }
}
