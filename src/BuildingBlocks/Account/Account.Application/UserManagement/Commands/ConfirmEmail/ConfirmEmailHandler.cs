using Microsoft.Extensions.Logging;
using Account.Application.IRepositories;
using Account.Application.IServices.Email;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Authorization;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace Account.Application.UserManagement.Commands.ConfirmEmail;

public class ConfirmEmailHandler(
    IUserWriteRepository _userWriteRepo,
    IEmailConfirmationTokenProvider _emailConfirmationTokenProvider,
    ILogger<ConfirmEmailHandler> _logger) : ICommandHandler<ConfirmEmailCommand>
{
    private const string ROLE_TO_ASSIGN = RoleCodes.USER;

    public async Task<UnitResult> Handle(ConfirmEmailCommand cmd, CancellationToken ct)
    {
        var validateTokenResult = _emailConfirmationTokenProvider
            .ValidateEmailConfirmationToken(cmd.EmailConfirmationToken);
        if (validateTokenResult.IsFailure)
            return UnitResult.Fail(validateTokenResult.Error);

        var userIdResult = _emailConfirmationTokenProvider
            .GetUserIdFromEmailConfirmationToken(cmd.EmailConfirmationToken);
        if(userIdResult.IsFailure)
            return UnitResult.Fail(userIdResult.Error); 

        var userId = userIdResult.Data!;

        var getUser = await _userWriteRepo.GetByIdAsync(UserId.Create(userId).Data!, ct);
        if (getUser.IsFailure)
            return UnitResult.Fail(getUser.Error);

        var user = getUser.Data!;

        user.ConfirmEmail();
        
        await _userWriteRepo.SaveChangesAsync(ct);

        _logger.LogInformation("Confirm email:{email} successful!", user.Email);

        return UnitResult.Ok();
    }
}

