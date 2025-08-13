using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Auth.Domain.Entities;
using PetFamily.Auth.Domain.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationConstants;

namespace PetFamily.Auth.Infrastructure.EFCore;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => PermissionId.Create(value).Data!
            ).
            HasColumnName("id");

        builder.Property(p => p.Code)
            .IsRequired()
            .HasColumnName("code")
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT);

        builder.Property(p => p.IsEnabled)
            .IsRequired()
            .HasColumnName("is_enabled");
    }
}
