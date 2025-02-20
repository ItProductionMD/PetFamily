﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetAggregates.Root;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;

namespace PetFamily.Infrastructure.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pets");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT);

        builder.Property(p => p.Color)
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT);

        builder.Property(p => p.DateTimeCreated)
            .IsRequired();

        builder.Property(p => p.DateOfBirth)
            .HasConversion(
                v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null, // Convert DateOnly to DateTime
                v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null         // Convert DateTime to DateOnly
            )
            .HasColumnType("date") // Map to the PostgreSQL DATE type
            .IsRequired(false);    // Adjust nullability as needed

        builder.Property(p => p.HelpStatus)
            .HasConversion<string>() // Store enum as a string, and get enum from string
            .IsRequired();

        // Value Objects
        builder.OwnsOne(p => p.PetType, petType =>
        {
            petType.Property(p => p.BreedId)
                .IsRequired();
            petType.Property(p => p.SpeciesId)
                .IsRequired();
        });

        builder.OwnsOne(p => p.OwnerPhone, phone =>
        {
            phone.Property(p => p.Number)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                .IsRequired();

            phone.Property(p => p.RegionCode)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                .IsRequired();
        });

        builder.OwnsOne(p => p.DonateDetails, donate =>
        {
            donate.Property(p => p.Name)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT);

            donate.Property(p => p.Description)
                .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT);
        });

        builder.OwnsOne(p => p.Adress, address =>
        {
            address.Property(a => a.Street)
                .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT);

            address.Property(a => a.City)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT);

            address.Property(a => a.Region)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT);
        });
        // For soft delete
        builder.Property<bool>("_isDeleted")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("is_deleted");

        builder.Property<DateTime?>("_deletedDateTime")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("deleted_date_time");
        // Indexes
        builder.HasIndex(p => p.Name).IsUnique(false);
    }
}
