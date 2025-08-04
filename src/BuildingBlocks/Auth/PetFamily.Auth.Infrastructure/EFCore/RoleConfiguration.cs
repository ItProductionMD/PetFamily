using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Auth.Domain;
using PetFamily.Auth.Domain.Entities.RoleAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationConstants;


namespace PetFamily.Auth.Infrastructure.EFCore;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => RoleId.Create(value).Data!)
            .HasColumnName("id");

        builder.Property(r => r.Code)
            .IsRequired()
            .HasColumnName("code")
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT);

        builder
            .HasMany<RolePermission>("_rolePermissions")
            .WithOne()
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

       // builder.Metadata
       //    .FindNavigation(nameof(Role.RolePermissions))!
       //    .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(r => r.RolePermissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

