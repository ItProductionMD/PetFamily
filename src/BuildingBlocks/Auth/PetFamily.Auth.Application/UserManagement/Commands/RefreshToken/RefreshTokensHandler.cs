using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Domain.Entities;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Error = PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Auth.Application.UserManagement.Commands.RefreshToken;

public class RefreshTokensHandler(
    IUserReadRepository userReadRepository,
    IRefreshTokenWriteRepository refreshTokenWriteRepository,
    IRoleReadRepository roleReadRepository,
    IJwtProvider tokenProvider,
    ILogger<RefreshTokensHandler> logger) : ICommandHandler<TokenResult, RefreshTokenCommand>
{
    private readonly IRefreshTokenWriteRepository _refreshTokenWriteRepository = refreshTokenWriteRepository;
    private readonly IUserReadRepository _userReadRepository = userReadRepository;
    private readonly IRoleReadRepository _roleReadRepository = roleReadRepository;
    private readonly IJwtProvider _tokenProvider = tokenProvider;
    private readonly ILogger<RefreshTokensHandler> _logger = logger;

    public async Task<Result<TokenResult>> Handle(
        RefreshTokenCommand cmd,
        CancellationToken ct)
    {
        var getRefreshToken = await _refreshTokenWriteRepository.GetRefreshToken(cmd.Token, ct);
        if (getRefreshToken.IsFailure)
            return Result.Fail(getRefreshToken.Error);

        var refreshToken = getRefreshToken.Data!;
        if (refreshToken.IsActive == false)
        {
            _logger.LogWarning("RefreshToken:{token} for userId:{userId} is not active!",
                 cmd.Token, refreshToken.UserId);

            return Result.Fail(Error.Authentication("Refresh token is not active!"));
        }
        var getJtiFromToken = _tokenProvider.GetJtiClaim(cmd.AccessToken);
        if (getJtiFromToken.IsFailure)
        {
            _logger.LogWarning("Get Jti From Token Failure!");
            return Result.Fail(getJtiFromToken.Error);
        }

        var jti = getJtiFromToken.Data!;
        var isParseJtiOk = Guid.TryParse(jti, out var jtiGuid);
        if (isParseJtiOk==false)
        {
            _logger.LogWarning("Parse Jti Error!");
            return Result.Fail(Error.Authentication("Parse Jti Error"));
        }

        if(refreshToken.Jti != jtiGuid)
        {
            _logger.LogWarning("Jti from access token doesn't correspond to Jti from refreshToken session");
            return Result.Fail(Error.Authentication("Jti does not correspond to existing"));
        }


        var validateFingerPrintResult = refreshToken.ValidateFingerPrint(cmd.FingerPrint);
        if (validateFingerPrintResult.IsFailure)
        {
            _logger.LogWarning("Validate fingerprint error!for token:{token}", cmd.Token);
            return Result.Fail(validateFingerPrintResult.Error);
        }

        var getUser = await _userReadRepository.GetByIdAsync(refreshToken.UserId, ct);
        if (getUser.IsFailure)
            return Result.Fail(getUser.Error);

        var user = getUser.Data!;

        var userId = UserId.Create(user.Id).Data!;

        var roleDtos = await _roleReadRepository.GetRolesByUserId(userId, ct);

        var permissionCodes = roleDtos
            .SelectMany(role => role.Permissions)
            .Select(permission => permission.PermissionCode)
            .ToList();

        var tokenResult = _tokenProvider.GenerateTokens(
            userId,
            user.Login,
            user.Email,
            user.Phone,
            permissionCodes);

        refreshToken.Revoke();

        var newRefreshToken = new RefreshTokenSession(
            Guid.NewGuid(),
            user.Id,
            tokenResult.RefreshToken,
            tokenResult.RefreshTokenExpiresAt,
            cmd.FingerPrint,
            tokenResult.Jti);

        await _refreshTokenWriteRepository.AddAsync(newRefreshToken, ct);

        await _refreshTokenWriteRepository.SaveChangesAsync(ct);

        return Result.Ok(tokenResult);
    }
}


