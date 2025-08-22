using Microsoft.Extensions.Logging;
using Account.Application.IRepositories;
using Account.Domain.Entities.UserAggregate;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using Volunteers.Public.IContracts;

namespace Account.Application.UserManagement.Commands.UpdateUser;

public class UpdateUserCommandHandler(
    IUserAccountUnitOfWork unitOfWork,
    IVolunteerPhoneUpdater volunteerPhoneUpdater,
    IUserWriteRepository userWriteRepo,
    ILogger<UpdateUserCommandHandler> logger) : ICommandHandler<UpdateUserProfileCommand>
{
    public async Task<UnitResult> Handle(UpdateUserProfileCommand cmd, CancellationToken ct)
    {
        // TODO: Validate command

        var getUser = await userWriteRepo.GetByIdAsync(UserId.Create(cmd.UserId).Data!, ct);
        if (getUser.IsFailure)
            return UnitResult.Fail(getUser.Error);

        var user = getUser.Data!;

        var socialNetworks = cmd.NewSocialNetworks
            .Select(sn => SocialNetworkInfo.Create(sn.Name, sn.Url).Data!)
            .ToList();

        var phoneResult = cmd.HasVolunteerAccount == false
            ? Phone.CreatePossibbleEmpty(cmd.NewPhoneNumber, cmd.NewPhoneNumber)
            : Phone.CreateNotEmpty(cmd.NewPhoneNumber, cmd.NewPhoneRegionCode);
        if (phoneResult.IsFailure)
            return UnitResult.Fail(phoneResult.Error);

        var phone = phoneResult.Data!;

        var profile = new Profile(cmd.NewLogin, phone, socialNetworks);

        user.UpdateProfile(profile);

        if (cmd.HasVolunteerAccount == false)
        {
            await userWriteRepo.SaveChangesAsync(ct);
            logger.LogInformation("User phone updated successfully!");

            return UnitResult.Ok();
        }

        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            await unitOfWork.SaveChangesAsync(ct);

            var updateResult = await volunteerPhoneUpdater.UpdatePhone(
                cmd.UserId,
                cmd.NewPhoneRegionCode,
                cmd.NewPhoneNumber,
                ct);
            if (updateResult.IsFailure)
            {
                logger.LogError("Volunteer phone update error: {Message}", updateResult.ToErrorMessage());
                await unitOfWork.RollbackAsync(ct);

                return UnitResult.Fail(updateResult.Error);
            }

            await unitOfWork.CommitAsync(ct);
            logger.LogInformation("User phone updated  successfully!");

            return UnitResult.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception during volunteer phone update");
            await unitOfWork.RollbackAsync(ct);

            return UnitResult.Fail(Error.InternalServerError("Update Volunteer phone error!"));
        }
    }

}
