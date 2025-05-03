using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.Shared.ValueObjects;
using Polly;
using System.Net.NetworkInformation;
using System.Text.Json;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Infrastructure.WriteDbConfigurations.Converters;

namespace PetFamily.Infrastructure.WriteDbConfigurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("pets");

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

        builder.ComplexProperty(p => p.PetType, petType =>
        {
            petType.Property(p => p.BreedId);
            petType.Property(p => p.SpeciesId);
        });

        builder.ComplexProperty(p => p.OwnerPhone, phone =>
        {
            phone.Property(p => p.Number)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT);

            phone.Property(p => p.RegionCode)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT);
        });

        builder.Property(p => p.Requisites)
            .HasConversion(new ReadOnlyListConverter<RequisitesInfo>())
            .HasColumnType("jsonb")
            .Metadata.SetValueComparer(new ReadOnlyListComparer<RequisitesInfo>());

        builder.ComplexProperty(p => p.Address, address =>
        {
            address.Property(a => a.Street)
                .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT);

            address.Property(a => a.City)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT);

            address.Property(a => a.Region)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT);

            address.Property(a => a.Number)
                .HasMaxLength (MAX_LENGTH_SHORT_TEXT);
        });

        builder.ComplexProperty(p => p.SerialNumber, v =>
        {
            v.Property(s => s.Value).HasColumnName("serial_number");
        });

        builder.Property(p => p.Images)
           .HasConversion(new ReadOnlyListConverter<Image>())
           .HasColumnType("jsonb")
           .Metadata.SetValueComparer(new ReadOnlyListComparer<Image>());

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
