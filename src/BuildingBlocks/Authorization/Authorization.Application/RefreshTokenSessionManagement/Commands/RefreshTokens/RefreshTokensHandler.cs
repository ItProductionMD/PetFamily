using Authorization.Application.Dtos;
using Authorization.Application.IRepositories.IRefreshTokenSessionRepo;
using Authorization.Application.IAuthorizationToken;
using Authorization.Domain.Entities;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using System.IdentityModel.Tokens.Jwt;
using Authorization.Public.Dtos;

namespace Authorization.Application.RefreshTokenSessionManagement.Commands.RefreshTokens;

public class RefreshTokensHandler(
    IRefreshTokenWriteRepo refreshTokenWriteRepo,
    IAuthorizationTokenProvider tokenProvider,
    ILogger<RefreshTokensHandler> logger) : ICommandHandler<AuthorizationTokens, RefreshTokenCommand>
{
    public async Task<Result<AuthorizationTokens>> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        //TODO use refreshTokenReadRepo instead of Write

        var getRefreshToken = await refreshTokenWriteRepo.GetRefreshTokenAsync(cmd.RefreshToken, ct);
        if (getRefreshToken.IsFailure)
            return Result.Fail(getRefreshToken.Error);

        var refreshTokenSession = getRefreshToken.Data!;
        if (refreshTokenSession.IsActive == false)
        {
            logger.LogWarning("RefreshToken:{token} for userId:{userId} is not active!",
                 cmd.RefreshToken, refreshTokenSession.UserId);

            return Result.Fail(Error.Authentication("Refresh token is not active!"));
        }

        var getJtiFromToken = tokenProvider.GetClaimValueByName(cmd.AccessToken, JwtRegisteredClaimNames.Jti);
        if (getJtiFromToken.IsFailure)
        {
            logger.LogWarning("Get Jti From Token Failure!");
            return Result.Fail(getJtiFromToken.Error);
        }

        var checkJtiResult = refreshTokenSession.CheckJti(getJtiFromToken.Data!);
        if (checkJtiResult.IsFailure)
        {
            var errorMessage = checkJtiResult.ToErrorMessage();
            logger.LogError("Error:{errorMessage}", errorMessage);

            return Result.Fail(Error.Authorization(errorMessage));
        }


        var validateFingerPrintResult = refreshTokenSession.ValidateFingerPrint(cmd.FingerPrint);
        if (validateFingerPrintResult.IsFailure)
        {
            logger.LogWarning("Validate fingerprint error!for token:{token}", cmd.RefreshToken);
            return Result.Fail(validateFingerPrintResult.Error);
        }
        var newRefreshTokenData = tokenProvider.GenerateRefreshTokenData();
        //TODO ADD CONTRACT FOR GETTING USER CLAIMS
        var newAccessTokenData = tokenProvider.GenerateAccessTokenData([]);

        refreshTokenSession.Revoke();

        var newJtiResult = tokenProvider.GetClaimValueByName(newAccessTokenData.AccessToken, JwtRegisteredClaimNames.Jti);
        if (newJtiResult.IsFailure)
            return Result.Fail(Error.InternalServerError("Error while getting jti from new access token!"));

        var newJti = Guid.Parse(newJtiResult.Data!);

        var newRefreshToken = new RefreshTokenSession(
            Guid.NewGuid(),
            refreshTokenSession.UserId,
            newRefreshTokenData.RefreshToken,
            newRefreshTokenData.ExpiresAt,
            cmd.FingerPrint,
            newJti);

        await refreshTokenWriteRepo.AddAndSaveAsync(newRefreshToken, ct);

        var tokenResult = new AuthorizationTokens(
            newAccessTokenData.AccessToken,
            newAccessTokenData.ExpiresAt,
            newRefreshTokenData.RefreshToken,
            newRefreshTokenData.ExpiresAt,
            newJti);

        return Result.Ok(tokenResult);
    }
}
