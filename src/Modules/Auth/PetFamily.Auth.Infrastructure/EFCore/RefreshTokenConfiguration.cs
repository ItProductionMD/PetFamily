using PetFamily.Auth.Domain.Entities;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.SharedInfrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PetFamily.Auth.Infrastructure.EFCore;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenSession>
{
    private const int TokenMaxLength = 200;

    public void Configure(EntityTypeBuilder<RefreshTokenSession> builder)
    {
        builder.ToTable("refresh_tokens", SchemaNames.AUTH);

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

