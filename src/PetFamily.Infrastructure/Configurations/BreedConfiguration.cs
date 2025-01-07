using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PetFamily.Domain.PetAggregates.Entities;
using static PetFamily.Domain.Shared.Constants;

namespace PetFamily.Infrastructure.Configurations;
public class BreedConfiguration : IEntityTypeConfiguration<Breed>
{
    public void Configure(EntityTypeBuilder<Breed> builder)
    {
        builder.ToTable("Breeds");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
            .IsRequired();

        builder.Property(b=>b.Description)
            .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT)
            .IsRequired(); 
    }
}
