using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Auth.Domain.Entities.RoleAggregate;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedInfrastructure.Shared.EFCore;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using static PetFamily.SharedInfrastructure.Shared.EFCore.Convertors;
using static PetFamily.SharedKernel.Validations.ValidationConstants;

namespace PetFamily.Auth.Infrastructure.EFCore;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder
            .HasKey(u => u.Id)
            .HasName("user_id");

        builder
            .Property(u => u.Id)
            .HasConversion(id => id.Value, value => UserId.Create(value).Data!);

        builder.Property(u => u.Login)
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
            .HasColumnName("login")
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
            .HasColumnName("email")
            .IsRequired();

        builder.Property(u => u.IsEmailConfirmed)
           .HasColumnName("is_email_confirmed")
           .IsRequired();

        builder.OwnsOne(user => user.Phone, phone =>
        {
            phone.Property(p => p.RegionCode)
               .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
               .HasColumnName("phone_region_code");

            phone.Property(p => p.Number)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                .HasColumnName("phone_number");
        });

        builder.Property(u => u.HashedPassword)
            .IsRequired()
            .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT)
            .HasColumnName("hashed_password");

        builder.Property(v => v.SocialNetworks)
          .HasConversion(new ReadOnlyListConverter<SocialNetworkInfo>())
          .HasColumnType("jsonb")
          .HasColumnName("social_networks")
          .Metadata.SetValueComparer(new ReadOnlyListComparer<SocialNetworkInfo>());

        builder.Property(u => u.LastLoginDate)
            .HasColumnName("last_login_date")
            .IsRequired(false);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(u => u.IsDeleted)
            .HasColumnName("is_deleted");

        builder.Property(u => u.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.Property(u => u.IsBlocked)
            .HasColumnName("is_blocked")
            .IsRequired();

        builder.Property(u => u.BlockedAt)
           .HasColumnName("blocked_at")
           .IsRequired(false);

        builder.Property(u => u.RoleId)
            .HasConversion(id => id.Value, value => RoleId.Create(value).Data!)
            .HasColumnName("role_id")
            .IsRequired(true);

        builder
           .HasOne<Role>()
           .WithMany()
           .HasForeignKey(u => u.RoleId)
           .OnDelete(DeleteBehavior.Restrict);

        builder.ApplyUniqueConstraints();
    }
}
