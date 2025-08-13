using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Commands.UpdateVolunteerRequest;

public class UpdateVolunteerRequestHandler(
    ILogger<UpdateVolunteerRequestHandler> logger,
    IVolunteerRequestWriteRepository requestRepo)
    : ICommandHandler<UpdateVolunteerRequestCommand>
{
    private readonly ILogger<UpdateVolunteerRequestHandler> _logger = logger;
    private readonly IVolunteerRequestWriteRepository _requestRepo = requestRepo;

    public async Task<UnitResult> Handle(UpdateVolunteerRequestCommand cmd, CancellationToken ct)
    {
        UpdateVolunteerRequestValidator.Validate(cmd);
      
        var userId = cmd.UserId;
       
        var getRequest = await _requestRepo.GetByIdAsync(cmd.VolunteerRequestId, ct);
        if (getRequest.IsFailure)
            return UnitResult.Fail(getRequest.Error);
        
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

        await _requestRepo.SaveAsync(ct);

        _logger.LogInformation("Volunteer request {RequestId} successfully updated", request.Id);

        return UnitResult.Ok();
    }
}

