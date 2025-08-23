using Authorization.Public.Contracts;
using Authorization.Public.Dtos;
using Microsoft.Extensions.Logging;
using Account.Application.Dtos;
using Account.Application.IRepositories;
using Account.Application.IServices;
using Account.Domain.Entities;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Authorization;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.SharedKernel.ValueObjects;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Account.Domain.Entities.UserAggregate;
using Account.Application.Extensions;


namespace Account.Application.UserManagement.Commands.LoginUserByEmail;

public class LoginByEmailCommandHandler(
    IUserWriteRepository userWriteRepo,
    IPasswordHasher passwordHasher,
    IUserAccountUnitOfWork authUnitOfWork,
    IAuthorizationTokenContract authorizationService,
    ILogger<LoginByEmailCommandHandler> logger) : ICommandHandler<AuthorizationTokens, LoginByEmailCommand>
{
    public async Task<Result<AuthorizationTokens>> Handle(LoginByEmailCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var userResult = await userWriteRepo.GetByEmailAsync(cmd.Email, ct);
        if (userResult.IsFailure)
            return Result.Fail(userResult.Error);

        var user = userResult.Data!;

        if (user.IsBlocked)
        {
            logger.LogWarning("User with email {Email} is blocked", cmd.Email);
            return Result.Fail(Error.Authentication("User is blocked"));
        }
        if(user.IsEmailConfirmed == false)
        {
            logger.LogWarning("User with email {Email} has unconfirmed email", cmd.Email);
            return Result.Fail(Error.Authentication("User has unconfirmed email"));
        }
        var verifyPasswordResult = passwordHasher.Verify(cmd.Password, user.HashedPassword);
        if (verifyPasswordResult.IsFailure)
        {
            user.ErrorAttemptLogin();

            await userWriteRepo.SaveChangesAsync(ct);

            var attemptCount = user.GetRemainingAttempts().ToString();

            logger.LogWarning("Password verification failed for user with email {Email} " +
                "remained attempts: {attemptCount}", cmd.Email, attemptCount);

            if (user.IsBlocked)
            {
                logger.LogWarning("User with email {Email} is blocked", cmd.Email);
                return Result.Fail(Error.Authentication("User is blocked"));
            }
            return Result.Fail(Error.Authentication("Password is wrong"));
        }

        var claims = user.CreateClaims();

        try
        {
            var tokensResult = await authorizationService.IssueAuthorizationTokensAsync(claims, ct);
            if(tokensResult.IsFailure)
            {
                logger.LogError("Error authorization service create :{Message}",tokensResult.ToErrorMessage());
                return Result.Fail(Error.InternalServerError("Create tokens error!"));
            }
            user.SetSuccessfulLogin();

            await authUnitOfWork.SaveChangesAsync(ct);

            return tokensResult;
        }

        catch (Exception ex)
        {
            await authorizationService.RevokeAuthorizationTokensAsync(user.Id.Value, CancellationToken.None);

            logger.LogCritical(ex, "AuthorizationService failed while creating tokens for user {UserId}", user.Id.Value);

            return Result.Fail(Error.InternalServerError(ex.Message));
        }
    }
}

