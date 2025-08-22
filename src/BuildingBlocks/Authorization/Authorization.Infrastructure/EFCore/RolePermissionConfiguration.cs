using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Authorization.Domain.Entities.RoleAggregate;
using Authorization.Domain.Entities;

namespace Authorization.Infrastructure.EFCore;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        //builder.Property(rp => rp.RoleId)
         //   .IsRequired()
           // .HasColumnName("role_id")
           // .HasConversion(
             //   id => id.Value,
              //  value => RoleId.Create(value).Data!);

        /*builder.Property(rp => rp.PermissionId)
            .IsRequired()
            .HasColumnName("permission_id")
            .HasConversion(
                id => id.Value,
                value => PermissionId.Create(value).Data!);*/

        builder.HasOne<Role>()
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rP => rP.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(rP => rP.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
