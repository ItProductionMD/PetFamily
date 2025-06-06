﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetSpecies.Domain;
using static PetFamily.SharedKernel.Validations.ValidationConstants;

namespace PetSpecies.Infrastructure.EFCore;
public class BreedConfiguration : IEntityTypeConfiguration<Breed>
{
    public void Configure(EntityTypeBuilder<Breed> builder)
    {
        builder.ToTable("breeds");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT)
            .IsRequired();
    }
}
