using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.ValueObjects.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Auth.Domain.Entities;
public class RefreshToken : Entity<Guid> // или AggregateRoot
{
    public UserId UserId { get; private set; }      // связь на агрегат User
    public string Token { get; private set; }       // само значение токена
    public DateTime ExpiresAt { get; private set; } // когда истекает
    public DateTime CreatedAt { get; private set; } // когда создан
    public DateTime? RevokedAt { get; private set; } // когда отозван

    private RefreshToken(Guid id) : base(id) { /* для EF */ }

    public RefreshToken(Guid id, UserId userId, string token, DateTime expiresAt) : base (id)
    {
        UserId = userId;
        Token = token;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;

    public void Revoke()
    {
        if (!IsActive)
            throw new InvalidOperationException("Token already inactive");
        RevokedAt = DateTime.UtcNow;
    }
}
