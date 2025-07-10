using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Auth.Public.Contracts;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;
using Volunteers.Public.Dto;
using Volunteers.Public.IContracts;

namespace PetFamily.VolunteerRequests.Application.Commands.ApproveVolunteerRequest;

public class ApproveVolunteerRequestHandler(
    IVolunteerRequestWriteRepository requestWriteRepository,
    IUserContext userContext,
    IUserFinder userFinder,
    IVolunteerCreator volunteerCreator,
    ILogger<ApproveVolunteerRequestHandler> logger) : ICommandHandler<ApproveVolunteerRequestCommand>
{
    private readonly IVolunteerRequestWriteRepository _requestWriteRepository = requestWriteRepository;
    private readonly IUserContext _userContext = userContext;
    private readonly IUserFinder _userFinder = userFinder;
    private readonly IVolunteerCreator _volunteerCreator = volunteerCreator;
    private readonly ILogger<ApproveVolunteerRequestHandler> _logger = logger;
    public async Task<UnitResult> Handle(ApproveVolunteerRequestCommand cmd, CancellationToken ct)
    {
        var getRequest = await _requestWriteRepository.GetByIdAsync(cmd.VolunteerRequestId, ct);
        if (getRequest.IsFailure)
        {
            return UnitResult.Fail(getRequest.Error);
        }
        var request = getRequest.Data!;

        var getAdmin = _userContext.GetUserId();
        if (getAdmin.IsFailure)
        {
            _logger.LogError("Failed to get user ID from context: {Error}", getAdmin.Error);
            return UnitResult.Fail(getAdmin.Error);
        }
        var adminId = getAdmin.Data!;

        var approveResult = request.Approve(adminId);
        if (approveResult.IsFailure)
        {
            _logger.LogWarning("Approve failed for request ID {Id}: {Error}",
                cmd.VolunteerRequestId, approveResult.Error);
            return UnitResult.Fail(approveResult.Error);
        }
        var saveResult = await _requestWriteRepository.SaveAsync(ct);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Failed to persist approved status for request ID {Id}: {Error}",
                cmd.VolunteerRequestId, saveResult.Error);
            return saveResult;
        }

        try
        {
            var getUser = await _userFinder.FindById(request.UserId, ct);
            if (getUser.IsFailure)
                throw new ApplicationException($"Failed to find user with ID {request.UserId}: {getUser.Error.Message}");

            var user = getUser.Data!;

            var createVolunteerDto = new CreateVolunteerDto(
                request.UserId,
                request.LastName,
                request.FirstName,
                user.Phone,
                request.ExperienceYears,
                request.Requisites);

            var createVolunteerResult = await _volunteerCreator.CreateVolunteer(createVolunteerDto, ct);
            if (createVolunteerResult.IsFailure)
                throw new ApplicationException($"Failed to create volunteer: {createVolunteerResult.Error.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating volunteer for request ID {Id}", cmd.VolunteerRequestId);

            var cancelResult = request.CancelApproval(adminId);
            if (cancelResult.IsSuccess)
            {
                var rollbackSaveResult = await _requestWriteRepository.SaveAsync(ct);
                if (rollbackSaveResult.IsFailure)
                {
                    _logger.LogError("Failed to rollback approved status for request ID {Id}: {Error}",
                        cmd.VolunteerRequestId, rollbackSaveResult.Error);
                    return UnitResult.Fail(rollbackSaveResult.Error);
                }
                _logger.LogInformation("Approval for request ID {Id} was rolled back.", cmd.VolunteerRequestId);
            }

            return UnitResult.Fail(Error.InternalServerError("Failed to create volunteer after approving request."));
        }

        _logger.LogInformation("Volunteer request with ID {Id} approved by admin {AdminId}",
            cmd.VolunteerRequestId, adminId);

        return UnitResult.Ok();
    }
}
