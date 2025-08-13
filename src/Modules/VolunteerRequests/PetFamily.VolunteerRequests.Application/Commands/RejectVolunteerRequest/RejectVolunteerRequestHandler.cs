using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Commands.RejectVolunteerRequest;

public class RejectVolunteerRequestHandler(
    IVolunteerRequestWriteRepository requestRepository,
    IUserContext userContext,
    ILogger<RejectVolunteerRequestHandler> logger) : ICommandHandler<RejectVolunteerRequestCommand>
{
    private readonly IVolunteerRequestWriteRepository _requestRepository = requestRepository;
    IUserContext _userContext = userContext;
    private readonly ILogger<RejectVolunteerRequestHandler> _logger = logger;

    public async Task<UnitResult> Handle(RejectVolunteerRequestCommand cmd, CancellationToken ct)
    {
        var getRequest = await _requestRepository.GetByIdAsync(cmd.VolunteerRequestId, ct);
        if (getRequest.IsFailure)
        {
            _logger.LogWarning("Failed to retrieve volunteer request with ID {VolunteerRequestId}. Error: {Error}",
                cmd.VolunteerRequestId, getRequest.Error);
            return UnitResult.Fail(getRequest.Error);
        }
        var request = getRequest.Data!;

        var adminId = _userContext.GetUserId();

        var rejectResult = request.Reject(adminId, cmd.Comment);
        if (rejectResult.IsFailure)
        {
            _logger.LogWarning("Reject failed for request ID {Id}: {Error}",
                cmd.VolunteerRequestId, rejectResult.Error);
            return UnitResult.Fail(rejectResult.Error);
        }

        await _requestRepository.SaveAsync(ct);

        _logger.LogInformation("Volunteer request with ID {Id} rejected by admin {AdminId}",
            cmd.VolunteerRequestId, adminId);

        return UnitResult.Ok();
    }
}
