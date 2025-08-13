using Microsoft.Extensions.Logging;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Authorization;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Auth.Application.UserManagement.Commands.ConfirmEmail;

public class ConfirmEmailHandler(
    IUserWriteRepository userWriteRepo,
    IEmailConfirmationService emailConfirmationService,
    IRoleReadRepository roleReadRepo,
    IRefreshTokenWriteRepository refreshTokenWriteRepository,
    ILogger<ConfirmEmailHandler> logger,
    IJwtProvider jwtProvider) : ICommandHandler<ConfirmEmailCommand>
{
    private readonly IRefreshTokenWriteRepository _refreshTokenWriteRepo = refreshTokenWriteRepository;
    private readonly IUserWriteRepository _userWriteRepo = userWriteRepo;
    private readonly ILogger<ConfirmEmailHandler> _logger = logger;
    private readonly IEmailConfirmationService _emailConfirmationService = emailConfirmationService;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IRoleReadRepository _roleReadRepo = roleReadRepo;
    private const string ROLE_TO_ASSIGN = RoleCodes.USER;

    public async Task<UnitResult> Handle(ConfirmEmailCommand cmd, CancellationToken ct)
    {
        var confirmationTokenResult = _emailConfirmationService.GetUserIdFromConfirmationToken(
            cmd.EmailConfirmationToken);
        if (confirmationTokenResult.IsFailure)
            return UnitResult.Fail(confirmationTokenResult.Error);

        var userId = UserId.Create(confirmationTokenResult.Data!).Data!;

        var getUser = await _userWriteRepo.GetByIdAsync(userId, ct);
        if (getUser.IsFailure)
            return UnitResult.Fail(getUser.Error);

        var user = getUser.Data!;

        var getRole = await _roleReadRepo.GetByCodeAsync(ROLE_TO_ASSIGN, ct);
        if (getRole.IsFailure)
            return UnitResult.Fail(getRole.Error);

        var roleDto = getRole.Data!;

        user.ConfirmEmail();

        user.ChangeRole(RoleId.Create(roleDto.RoleId).Data!);

        await _userWriteRepo.SaveChangesAsync(ct);

        _logger.LogInformation("Confirm email:{email} successful!", user.Email);

        return UnitResult.Ok();
    }
}

