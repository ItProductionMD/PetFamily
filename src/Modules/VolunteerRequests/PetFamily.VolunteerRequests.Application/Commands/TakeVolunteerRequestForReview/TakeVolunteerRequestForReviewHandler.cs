using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Discussions.Public.Contracts;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Enums;

namespace PetFamily.VolunteerRequests.Application.Commands.TakeVolunteerRequestForReview;

public class TakeVolunteerRequestForReviewHandler(
    IVolunteerRequestWriteRepository requestRepository,
    IDiscussionCreator discussionCreator,
    IDiscussionRemover discussionRemover,
    IUserContext userContext,
    ILogger<TakeVolunteerRequestForReviewHandler> logger) : ICommandHandler<TakeVolunteerRequestForReviewCommand>
{
    private readonly IVolunteerRequestWriteRepository _requestRepository = requestRepository;
    private readonly ILogger<TakeVolunteerRequestForReviewHandler> _logger = logger;
    private readonly IUserContext _userContext = userContext;
    private readonly IDiscussionRemover _discussionRemover = discussionRemover;
    private readonly IDiscussionCreator _discussionCreator = discussionCreator;

    public async Task<UnitResult> Handle(TakeVolunteerRequestForReviewCommand cmd, CancellationToken ct)
    {
        TakeVolunteerRequestForReviewValidator.Validate(cmd);
        
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
        
        var adminId = _userContext.GetUserId();

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
        try
        {
            await _requestRepository.SaveAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save volunteer request with ID {VolunteerRequestId}. Error: {Message}",
                volunteerRequest.Id, ex.Message);

            await _discussionRemover.RemoveDisscusion(discussionId);

            return UnitResult.Fail(Error.InternalServerError("Failed to save volunteer request after taking it for review."));
        }
        _logger.LogInformation("Volunteer request with ID {VolunteerRequestId} has been taken for review by admin ID {AdminId}.",
            volunteerRequest.Id, adminId);

        return UnitResult.Ok();
    }
}
