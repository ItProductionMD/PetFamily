using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Commands.UpdateVolunteerRequest;

public class UpdateVolunteerRequestHandler(
    ILogger<UpdateVolunteerRequestHandler> logger,
    IUserContext userContext,
    IVolunteerRequestWriteRepository requestRepository)
    : ICommandHandler<UpdateVolunteerRequestCommand>
{
    private readonly ILogger<UpdateVolunteerRequestHandler> _logger = logger;
    private readonly IUserContext _userContext = userContext;
    private readonly IVolunteerRequestWriteRepository _requestRepository = requestRepository;

    public async Task<UnitResult> Handle(UpdateVolunteerRequestCommand cmd, CancellationToken ct)
    {
        var validationResult = UpdateVolunteerRequestValidator.Validate(cmd);
        if (validationResult.IsFailure)
        {
            _logger.LogWarning("Validation failed for UpdateVolunteerRequestCommand: {Errors}",
                validationResult.ValidationMessagesToString());
            return validationResult;
        }

        var getUserId = _userContext.GetUserId();
        if (getUserId.IsFailure)
        {
            _logger.LogError("Failed to get user ID from context: {Error}", getUserId.Error);
            return UnitResult.Fail(getUserId.Error);
        }
        var userId = getUserId.Data!;

        var getRequest = await _requestRepository.GetByIdAsync(cmd.VolunteerRequestId, ct);
        if (getRequest.IsFailure)
        {
            _logger.LogWarning("Volunteer request with ID {Id} not found", cmd.VolunteerRequestId);
            return UnitResult.Fail(getRequest.Error);
        }
        var request = getRequest.Data!;

        if (request.UserId != userId)
        {
            _logger.LogWarning("User {UserId} tried to update someone else's volunteer request " +
                "{RequestId}",userId, request.Id);
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
            _logger.LogWarning("Failed to update volunteer request {RequestId}: {Error}",
                cmd.VolunteerRequestId, updateResult.Error);
            return UnitResult.Fail(updateResult.Error);
        }
        var saveResult = await _requestRepository.SaveAsync(ct);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Failed to save updated request {RequestId}: {Error}", 
                request.Id, saveResult.Error);
            return saveResult;
        }

        _logger.LogInformation("Volunteer request {RequestId} successfully updated", request.Id);

        return UnitResult.Ok();
    }
}

