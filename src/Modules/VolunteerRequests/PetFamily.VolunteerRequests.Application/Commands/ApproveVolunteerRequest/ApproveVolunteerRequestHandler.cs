using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Auth.Public.Contracts;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;
using Volunteers.Public.Dto;
using Volunteers.Public.IContracts;

namespace PetFamily.VolunteerRequests.Application.Commands.ApproveVolunteerRequest;

public class ApproveVolunteerRequestHandler(
    IVolunteerRequestWriteRepository requestWriteRepo,
    IUserContract userFinder,
    IVolunteerCreator volunteerCreator,
    ILogger<ApproveVolunteerRequestHandler> logger) : ICommandHandler<ApproveVolunteerRequestCommand>
{
    private readonly IVolunteerRequestWriteRepository _requestWriteRepo = requestWriteRepo;
    private readonly IUserContract _userFinder = userFinder;
    private readonly IVolunteerCreator _volunteerCreator = volunteerCreator;
    private readonly ILogger<ApproveVolunteerRequestHandler> _logger = logger;

    public async Task<UnitResult> Handle(ApproveVolunteerRequestCommand cmd, CancellationToken ct)
    {
        var adminId = cmd.AdminId;

        var getRequest = await _requestWriteRepo.GetByIdAsync(cmd.VolunteerRequestId, ct);
        if (getRequest.IsFailure)
            return UnitResult.Fail(getRequest.Error);
        
        var request = getRequest.Data!;

        var approveResult = request.Approve(adminId);
        if (approveResult.IsFailure)
        {
            _logger.LogWarning("Approve failed for request ID {Id}: {Error}",
                cmd.VolunteerRequestId, approveResult.Error);

            return UnitResult.Fail(approveResult.Error);
        }

        await _requestWriteRepo.SaveAsync(ct);

        try
        {
            var getUser = await _userFinder.GetByIdAsync(request.UserId, ct);
            if (getUser.IsFailure)
                throw new ApplicationException($"Failed to find user with ID {request.UserId}: " +
                    $"{getUser.Error.Message}");

            var user = getUser.Data!;

            var createVolunteerDto = new CreateVolunteerDto(
                adminId,
                request.UserId,
                request.LastName,
                request.FirstName,
                user.Phone,
                request.ExperienceYears,
                request.Requisites);

            var createVolunteerResult = await _volunteerCreator.CreateVolunteer(createVolunteerDto, ct);
            if (createVolunteerResult.IsFailure)
                throw new ApplicationException($"Failed to create volunteer: {createVolunteerResult.Error.Message}");
                         
            _logger.LogInformation("Volunteer request with ID {Id} approved by admin {AdminId} successful!",
            cmd.VolunteerRequestId, adminId);

            return UnitResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating volunteer for request ID {Id}", cmd.VolunteerRequestId);

            var cancelResult = request.CancelApproval(adminId);
            if (cancelResult.IsSuccess)
            {
                await _requestWriteRepo.SaveAsync(ct);

                _logger.LogInformation("Approval for request ID {Id} was rolled back.", cmd.VolunteerRequestId);
                return UnitResult.Fail(Error.InternalServerError("Failed to create volunteer after approving request."));
            }
            else
            {
                _logger.LogCritical("Error cancel approval for request with id:{Id}", cmd.VolunteerRequestId);
                return UnitResult.Fail(Error.InternalServerError("Failed to cancel request approval"));
            }
        }     
    }
}
