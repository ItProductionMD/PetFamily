using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PetFamily.Domain.PetAggregates.Entities;
using static PetFamily.Domain.Shared.Constants;

namespace PetFamily.Infrastructure.Configurations;
public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.ToTable("Species");

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
