using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using static PetFamily.SharedKernel.Authorization.PermissionCodes.VolunteerManagement;

namespace PetFamily.Auth.Application.UserManagement.Commands.UpdateUser;

public class UpdateUserCommandHandler(
    IPasswordHasher passwordHasher,
    IUserContext userContext,
    ILogger<UpdateUserCommandHandler> logger,
    IAuthUnitOfWork unitOfWork,
    IUserWriteRepository userWriteRepository) : ICommandHandler<UpdateUserProfileCommand>
{
    private readonly IAuthUnitOfWork _authUnitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUserContext _userContext = userContext;

    private readonly IUserWriteRepository _userWriteRepository = userWriteRepository;
    public async Task<UnitResult> Handle(UpdateUserProfileCommand cmd, CancellationToken ct)
    {
        //TODO Validate command
        var userId = _userContext.GetUserId();

        var getUser = await _userWriteRepository.GetByIdAsync(UserId.Create(userId).Data!, ct);
        if (getUser.IsFailure)
            return UnitResult.Fail(getUser.Error);

        var user = getUser.Data!;

        var hasVolunteerRole = _userContext.HasPermission(VolunteerEdit);

        var socialNetworks = cmd.NewSocialNetworks
            .Select(sn => SocialNetworkInfo.Create(sn.Name, sn.Url).Data!)
            .ToList();

        var phone = Phone.CreateEmpty();

        if (hasVolunteerRole)
        {
            var phoneResult = Phone.CreateNotEmpty(cmd.NewPhoneNumber, cmd.NewPhoneRegionCode);
            if (phoneResult.IsFailure)
                return Result.Fail(phoneResult.Error);

            phone = phoneResult.Data!;
        }
        else
        {
            var phoneResult = Phone.CreatePossibbleEmpty(cmd.NewPhoneNumber, cmd.NewPhoneNumber);
            if (phoneResult.IsFailure)
                return Result.Fail(phoneResult.Error);

            phone = phoneResult.Data!;
        }

        var profile = new Profile(cmd.NewLogin, phone, socialNetworks);

        user.UpdateProfile(profile);


        //TODO NEED EVENT SOURCING TO UPDATE VOLUNTEER

        throw new NotImplementedException();
    }
}
