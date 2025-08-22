using Authorization.Public.Dtos;
using PetFamily.SharedKernel.Results;
using System.Security.Claims;

namespace Authorization.Public.Contracts;

public interface IAuthorizationTokenContract
{
    Task<Result<AuthorizationTokens>> IssueAuthorizationTokensAsync(IEnumerable<Claim> claims, CancellationToken ct = default);
    Task RevokeAuthorizationTokensAsync(Guid userId, CancellationToken ct = default);
}
