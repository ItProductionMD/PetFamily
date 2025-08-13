using Microsoft.Extensions.Logging;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using Volunteers.Public.IContracts;

namespace PetFamily.Auth.Application.UserManagement.Commands.UpdateUser;

public class UpdateUserCommandHandler(
    IPasswordHasher passwordHasher,
    ILogger<UpdateUserCommandHandler> logger,
    IAuthUnitOfWork unitOfWork,
    IVolunteerPhoneUpdater volunteerPhoneUpdater,
    IUserWriteRepository userWriteRepo) : ICommandHandler<UpdateUserProfileCommand>
{
    private readonly ILogger<UpdateUserCommandHandler> _logger = logger;
    private readonly IAuthUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUserWriteRepository _userWriteRepo = userWriteRepo;
    private readonly IVolunteerPhoneUpdater _volunteerPhoneUpdater = volunteerPhoneUpdater;
    public async Task<UnitResult> Handle(UpdateUserProfileCommand cmd, CancellationToken ct)
    {
        // TODO: Validate command

        var getUser = await _userWriteRepo.GetByIdAsync(UserId.Create(cmd.UserId).Data!, ct);
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

        var phone = phoneResult.Data;

        var profile = new Profile(cmd.NewLogin, phone, socialNetworks);

        user.UpdateProfile(profile);

        if (cmd.HasVolunteerAccount == false)
        {
            await _userWriteRepo.SaveChangesAsync(ct);
            _logger.LogInformation("User phone updated successfully!");

            return UnitResult.Ok();
        }

        await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);

            var updateResult = await _volunteerPhoneUpdater.UpdatePhone(
                cmd.UserId,
                cmd.NewPhoneRegionCode,
                cmd.NewPhoneNumber,
                ct);
            if (updateResult.IsFailure)
            {
                _logger.LogError("Volunteer phone update error: {Message}", updateResult.ToErrorMessage());
                await _unitOfWork.RollbackAsync(ct);

                return UnitResult.Fail(updateResult.Error);
            }

            await _unitOfWork.CommitAsync(ct);
            _logger.LogInformation("User phone updated  successfully!");

            return UnitResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during volunteer phone update");
            await _unitOfWork.RollbackAsync(ct);

            return UnitResult.Fail(Error.InternalServerError("Update Volunteer phone error!"));
        }
    }

}
