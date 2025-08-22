using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PetFamily.SharedInfrastructure.Constants;
using Authorization.Domain.Entities;

namespace Authorization.Infrastructure.EFCore;

public class RefreshTokenSessionConfiguration : IEntityTypeConfiguration<RefreshTokenSession>
{
    private const int TokenMaxLength = 200;

    public void Configure(EntityTypeBuilder<RefreshTokenSession> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id)
               .HasColumnName("id")
               .ValueGeneratedNever();

        builder.Property(rt => rt.UserId)
               .HasColumnName("user_id")
               .IsRequired();

        builder.Property(rt => rt.Token)
               .HasColumnName("token")
               .IsRequired()
               .HasMaxLength(TokenMaxLength);

        builder.Property(rt => rt.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired();

        builder.Property(rt => rt.ExpiresAt)
               .HasColumnName("expires_at")
               .IsRequired();

        builder.Property(rt => rt.RevokedAt)
               .HasColumnName("revoked_at");

        builder.Ignore(rt => rt.IsActive);
    }
}
