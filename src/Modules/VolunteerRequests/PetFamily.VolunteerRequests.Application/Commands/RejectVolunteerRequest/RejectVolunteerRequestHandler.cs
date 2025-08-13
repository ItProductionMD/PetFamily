using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Commands.RejectVolunteerRequest;

public class RejectVolunteerRequestHandler(
    IVolunteerRequestWriteRepository requestRepo,
    ILogger<RejectVolunteerRequestHandler> logger) : ICommandHandler<RejectVolunteerRequestCommand>
{
    private readonly IVolunteerRequestWriteRepository _requestRepo = requestRepo;
    private readonly ILogger<RejectVolunteerRequestHandler> _logger = logger;

    public async Task<UnitResult> Handle(RejectVolunteerRequestCommand cmd, CancellationToken ct)
    {
        var adminId = cmd.AdminId; ;

        var getRequest = await _requestRepo.GetByIdAsync(cmd.VolunteerRequestId, ct);
        if (getRequest.IsFailure)   
            return UnitResult.Fail(getRequest.Error);
        
        var request = getRequest.Data!;

        var rejectResult = request.Reject(adminId, cmd.Comment);
        if (rejectResult.IsFailure)
        {
            _logger.LogWarning("Reject failed for request ID {Id}: {Error}",
                cmd.VolunteerRequestId, rejectResult.Error);
            return UnitResult.Fail(rejectResult.Error);
        }

        await _requestRepo.SaveAsync(ct);

        _logger.LogInformation("Volunteer request with ID {Id} rejected by admin {AdminId}",
            cmd.VolunteerRequestId, adminId);

        return UnitResult.Ok();
    }
}
