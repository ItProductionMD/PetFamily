using Authorization.Public.Contracts;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using Volunteers.Application.IRepositories;
using Volunteers.Domain;
using Volunteers.Domain.ValueObjects;
using static PetFamily.SharedKernel.Authorization.RoleCodes;

namespace Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

public class CreateVolunteerHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    IRoleContract roleContract,
    ILogger<CreateVolunteerHandler> logger) : ICommandHandler<Guid, CreateVolunteerCommand>
{

    public async Task<Result<Guid>> Handle(CreateVolunteerCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var volunteer = CreateVolunteerProcess(cmd);

        var addResult = await volunteerWriteRepo.AddAndSaveAsync(volunteer, ct);
        if (addResult.IsFailure)
            return addResult;
        try
        {
            var result = await roleContract.AssignRole(cmd.UserId, VOLUNTEER, ct);
            if(result.IsFailure)
            {
                logger.LogWarning("Failed to change role for user with id:{UserId} to {RoleCode}: {Error}",
                    cmd.UserId, VOLUNTEER, result.Error);
                return Result.Fail(result.Error);
            }
            logger.LogInformation("Volunteer with id:{Id},created successful by admin with id:{adminId}!",
            volunteer.Id, cmd.AdminId);

            return Result.Ok(volunteer.Id);
        }
        catch(Exception ex)
        {
            // If role assignment fails, we need to clean up the volunteer record
            await volunteerWriteRepo.Delete(volunteer, CancellationToken.None);

            logger.LogWarning("Failed to change role for user with id:{UserId} to {RoleCode}: {Message}",
                    cmd.UserId, VOLUNTEER, ex.Message);

            var errorResult = Error.InternalServerError(
                $"Failed to change role for user with id:{cmd.UserId} to {VOLUNTEER}: {ex.Message}");
            return Result.Fail(errorResult);
        }
    }

    private static Volunteer CreateVolunteerProcess(CreateVolunteerCommand cmd)
    {
        var phone = Phone.CreateNotEmpty(cmd.PhoneNumber, cmd.PhoneRegionCode).Data!;

        var fullName = FullName.Create(cmd.FirstName, cmd.LastName).Data!;

        var requisites = cmd.Requisites
            .Select(dto => RequisitesInfo.Create(dto.Name, dto.Description).Data!).ToList();

        return Volunteer.Create(
            VolunteerID.NewGuid(),
            UserId.Create(cmd.UserId).Data!,
            fullName,
            cmd.ExperienceYears,
            cmd.Description,
            phone,
            requisites).Data!;
    }
}
