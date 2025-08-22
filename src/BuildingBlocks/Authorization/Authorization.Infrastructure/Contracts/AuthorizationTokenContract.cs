using Authorization.Public.Contracts;
using Authorization.Public.Dtos;
using PetFamily.SharedKernel.Results;
using System.Security.Claims;

namespace Authorization.Infrastructure.Contracts;

internal class AuthorizationTokenContract : IAuthorizationTokenContract
{
    public Task<Result<AuthorizationTokens>> IssueAuthorizationTokensAsync(IEnumerable<Claim> claims, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task RevokeAuthorizationTokensAsync(Guid userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
