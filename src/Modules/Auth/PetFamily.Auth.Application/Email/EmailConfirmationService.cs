using PetFamily.Auth.Application.Constants;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using System.Security.Claims;

namespace PetFamily.Auth.Application.Email;

public class EmailConfirmationService(
    IEmailService emailService,
    IJwtProvider jwtProvider) : IEmailConfirmationService
{
    private readonly IEmailService _emailService = emailService;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task SendEmailConfirmationMessage(User user, CancellationToken ct = default)
    {
        var emailTokenOptions = new EmailConfirmationToken(user);

        var emailConfirmationToken = _jwtProvider.GenerateJwtToken(
            emailTokenOptions.claims,
            emailTokenOptions.IssueDay,
            emailTokenOptions.ExpiredAt).Token;

        var emailConfirmationMessage = EmailMessage.CreateConfirmationEmailMessage(
            user.Email,
            emailConfirmationToken);

        await _emailService.SendAsync(emailConfirmationMessage, ct);
    }

    public Result<Guid> GetUserIdFromConfirmationToken(string emailConfirmationToken)
    {
        var validateEmailConfirmToken = _jwtProvider.ValidateToken(emailConfirmationToken);
        if (validateEmailConfirmToken.IsFailure)
            return UnitResult.Fail(validateEmailConfirmToken.Error);

        var principals = validateEmailConfirmToken.Data!;

        var reasonClaim = principals.FindFirst(CustomClaimTypes.REASON);
        if (reasonClaim == null || reasonClaim.Value != EmailConfirmationToken.CLAIM_REASON)
            return UnitResult.Fail(Error.Authentication("reason claim not found"));

        var userIdClaim = principals.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return UnitResult.Fail(Error.Authentication("UserId claim not found"));

        var userId = userIdClaim.Value;

        var isUserIdParsedOk = Guid.TryParse(userId, out Guid userIdGuid);
        if (isUserIdParsedOk == false)
            return UnitResult.Fail(Error.Authentication("UserId claim not valid format"));

        return Result.Ok(userIdGuid);
    }
}
