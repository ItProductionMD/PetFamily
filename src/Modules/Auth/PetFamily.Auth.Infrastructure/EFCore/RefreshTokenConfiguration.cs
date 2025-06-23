using PetFamily.Auth.Domain.Entities;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.SharedInfrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PetFamily.Auth.Infrastructure.EFCore;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    private const int TokenMaxLength = 200;

    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Таблица и схема
        builder.ToTable("refresh_tokens", SchemaNames.AUTH);

        // Первичный ключ
        builder.HasKey(rt => rt.Id)
               .HasName("PK_RefreshTokens");

        // Свойство Id (Guid)
        builder.Property(rt => rt.Id)
               .HasColumnName("id")
               .ValueGeneratedNever();

        // Свойство UserId (Guid, внешний ключ)
        builder.Property(rt => rt.UserId)
               .HasColumnName("user_id")
               .IsRequired();

        builder.HasOne<User>()                     // связь с User
               .WithMany()                          // Assuming User.RefreshTokens exists; otherwise skip navigation
               .HasForeignKey(rt => rt.UserId)
               .HasConstraintName("FK_RefreshTokens_Users");

        // Сам токен
        builder.Property(rt => rt.Token)
               .HasColumnName("token")
               .IsRequired()
               .HasMaxLength(TokenMaxLength);

        // Время создания
        builder.Property(rt => rt.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired();

        // Время истечения
        builder.Property(rt => rt.ExpiresAt)
               .HasColumnName("expires_at")
               .IsRequired();

        // Время отзыва (nullable)
        builder.Property(rt => rt.RevokedAt)
               .HasColumnName("revoked_at");

        // Вычисляемое свойство IsActive — не мапим, если оно в домене вычисляется
        builder.Ignore(rt => rt.IsActive);
    }
}

