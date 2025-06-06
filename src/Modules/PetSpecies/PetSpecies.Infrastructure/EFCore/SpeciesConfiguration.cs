﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetSpecies.Domain;
using System.Reflection.Emit;
using static PetFamily.SharedKernel.Validations.ValidationConstants;

namespace PetSpecies.Infrastructure.EFCore;
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
