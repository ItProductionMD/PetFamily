using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using PetFamily.Domain.PetTypeManagment.Entities;

namespace PetFamily.Infrastructure.WriteDbConfigurations;
public class BreedConfiguration : IEntityTypeConfiguration<Breed>
{
    public void Configure(EntityTypeBuilder<Breed> builder)
    {
        builder.ToTable("breeds");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
            .IsRequired();

        builder.Property(b=>b.Description)
            .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT)
            .IsRequired(); 
    }
}
