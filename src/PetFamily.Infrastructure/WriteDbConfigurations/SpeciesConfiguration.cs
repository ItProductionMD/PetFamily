using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using PetFamily.Domain.PetManagment.Entities;

namespace PetFamily.Infrastructure.WriteDbConfigurations;
public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.ToTable("species");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
            .IsRequired();
            
        // One-to-many relationship with Breeds
        builder.HasMany(s => s.Breeds)
            .WithOne()
            .HasForeignKey("SpeciesId") // Create a FK in Breed table pointing to Species
            .OnDelete(DeleteBehavior.Cascade);
    }
}
