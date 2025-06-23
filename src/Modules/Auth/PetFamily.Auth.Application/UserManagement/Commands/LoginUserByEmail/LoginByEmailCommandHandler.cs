using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Application.IServices;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;


namespace PetFamily.Auth.Application.UserManagement.Commands.LoginUserByEmail;

public class LoginByEmailCommandHandler(
    IUserWriteRepository userWriteRepository,
    IRoleReadRepository roleRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    ILogger<LoginByEmailCommandHandler> logger) : ICommandHandler<TokenResult, LoginByEmailCommand>
{
    private readonly IUserWriteRepository _userWriteRepository = userWriteRepository;
    private readonly IRoleReadRepository _roleRepository = roleRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<LoginByEmailCommandHandler> _logger = logger;

    public async Task<Result<TokenResult>> Handle(LoginByEmailCommand cmd, CancellationToken ct)
    {
        var validateCommandResult = LoginByEmailCommandValidator.Validate(cmd);
        if (validateCommandResult.IsFailure)
        {
            _logger.LogWarning("VALIDATION FAILED for LoginByEmailCommand: {Errors}",
            validateCommandResult.ValidationMessagesToString());

            return validateCommandResult;
        }
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
        var roleDtos = await _roleRepository.GetRolesByUserId(user.Id, ct);

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

        user.LoginAttempts = 0;

        user.LastLoginDate = DateTime.UtcNow;

        await _userWriteRepository.SaveChangesAsync(ct);

        // TODO: optionally save refreshToken to DB here

        return Result.Ok(tokens);
    }
}

