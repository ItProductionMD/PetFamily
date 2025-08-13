using Microsoft.Extensions.Logging;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Domain.Entities;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;


namespace PetFamily.Auth.Application.UserManagement.Commands.LoginUserByEmail;

public class LoginByEmailCommandHandler(
    IUserWriteRepository userWriteRepository,
    IRoleReadRepository roleRepository,
    IRefreshTokenWriteRepository refreshTokenWriteRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IAuthUnitOfWork authUnitOfWork,
    ILogger<LoginByEmailCommandHandler> logger) : ICommandHandler<TokenResult, LoginByEmailCommand>
{
    private readonly IUserWriteRepository _userWriteRepository = userWriteRepository;
    private readonly IRoleReadRepository _roleReadRepository = roleRepository;
    private readonly IRefreshTokenWriteRepository _refreshTokenWriteRepository = refreshTokenWriteRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<LoginByEmailCommandHandler> _logger = logger;
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork;

    public async Task<Result<TokenResult>> Handle(LoginByEmailCommand cmd, CancellationToken ct)
    {
        LoginByEmailCommandValidator.Validate(cmd);

        var user = await _userWriteRepository.GetByEmailAsync(cmd.Email, ct);
        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found", cmd.Email);
            return Result.Fail(Error.Authentication("User Not Found"));
        }
        if (user.IsBlocked)
        {
            _logger.LogWarning("User with email {Email} is blocked", cmd.Email);

            return Result.Fail(Error.Authentication("User is blocked"));
        }

        var verifyPasswordResult = _passwordHasher.Verify(cmd.Password, user.HashedPassword);
        if (verifyPasswordResult.IsFailure)
        {
            user.ErrorAttemptLogin();

            await _userWriteRepository.SaveChangesAsync(ct);

            var attemptCount = user.GetRemainingAttempts().ToString();

            _logger.LogWarning("Password verification failed for user with email {Email} " +
                "remained attempts: {attemptCount}", cmd.Email, attemptCount);

            if (user.IsBlocked)
            {
                _logger.LogWarning("User with email {Email} is blocked", cmd.Email);
                return Result.Fail(Error.Authentication("User is blocked"));
            }
            return Result.Fail(Error.Authentication("Password is wrong"));
        }
        var roleDtos = await _roleReadRepository.GetRolesByUserId(user.Id, ct);

        var permissionCodes = roleDtos
            .SelectMany(role => role.Permissions)
            .Select(permission => permission.PermissionCode)
            .ToList();

        var tokens = _jwtProvider.GenerateTokens(
            user.Id,
            user.Login,
            user.Email,
            user.Phone?.ToString() ?? string.Empty,
            permissionCodes
        );

        user.SetSuccessfulLogin();

        var refreshTokenSession = new RefreshTokenSession(
            Guid.NewGuid(),
            user.Id.Value,
            tokens.RefreshToken,
            tokens.RefreshTokenExpiresAt,
            cmd.Fingerprint,
            tokens.Jti);

        await _refreshTokenWriteRepository.AddAsync(refreshTokenSession, ct);

        await _authUnitOfWork.SaveChangesAsync(ct);

        return Result.Ok(tokens);
    }
}

