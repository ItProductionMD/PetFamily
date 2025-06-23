using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Authorization;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Auth.Application.UserManagement.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler(
    IUserWriteRepository userWriteRepository,
    IEmailConfirmationService emailConfirmationService,
    IRoleReadRepository roleReadRepository,
    IJwtProvider jwtProvider
    ) : ICommandHandler<TokenResult, ConfirmEmailCommand>
{
    private readonly IUserWriteRepository _userWriteRepository = userWriteRepository;
    private readonly IEmailConfirmationService _emailConfirmationService = emailConfirmationService;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IRoleReadRepository _roleReadRepository = roleReadRepository;
    private const string ROLE_TO_ASSIGN = RoleCodes.VOLUNTEER;

    public async Task<Result<TokenResult>> Handle(ConfirmEmailCommand cmd, CancellationToken ct)
    {
        var confirmationTokenResult = _emailConfirmationService.GetUserIdFromConfirmationToken(
            cmd.EmailConfirmationToken);
        if (confirmationTokenResult.IsFailure)
            return Result.Fail(confirmationTokenResult.Error);

        var userId = UserId.Create(confirmationTokenResult.Data!).Data!;

        var getUser = await _userWriteRepository.GetByIdAsync(userId, ct);
        if (getUser.IsFailure)
            return Result.Fail(getUser.Error);

        var user = getUser.Data!;

        var getRole = await _roleReadRepository.GetByCodeAsync(ROLE_TO_ASSIGN, ct);
        if (getRole.IsFailure)
            return Result.Fail(getRole.Error);

        var roleDto = getRole.Data!;

        user.ConfirmEmail();

        user.UpdateRoles([RoleId.Create(roleDto.RoleId).Data!]);

        await _userWriteRepository.SaveChangesAsync(ct);

        var permissionCodes = roleDto.Permissions.Select(p => p.PermissionCode).ToList();

        var tokens = _jwtProvider.GenerateTokens(
            user.Id,
            user.Login,
            user.Email,
            user.Phone?.ToString() ?? string.Empty,
            permissionCodes);

        return Result.Ok(tokens);
    }
}

