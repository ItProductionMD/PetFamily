using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Auth.Domain.Entities.RoleAggregate;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Auth.Infrastructure.EFCore;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_role");

        builder.HasKey(uR => new { uR.UserId, uR.RoleId });

        builder.Property(uR => uR.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value).Data!)
            .HasColumnName("user_id");

        builder.Property(uR => uR.RoleId)
            .HasConversion(
                id => id.Value,
                value => RoleId.Create(value).Data!)
            .HasColumnName("role_id");

        builder.HasOne<User>()
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
