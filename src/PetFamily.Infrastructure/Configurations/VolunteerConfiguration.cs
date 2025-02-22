﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Shared.ValueObjects;
using System.Reflection.Emit;
using System.Text.Json;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Infrastructure.Configurations.Converters;

namespace PetFamily.Infrastructure.Configurations;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id);

        builder.Property(v => v.Email)
            .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT)
            .IsRequired();

        builder.Property(v => v.ExperienceYears);

        builder.Property(v => v.Description)
            .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT);

        // Value Objects
        builder.ComplexProperty(v => v.FullName,fn =>
        {
            fn.Property(f => f.FirstName)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                .HasColumnName("first_name");

            fn.Property(f => f.LastName)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                .HasColumnName("last_name");
        });

        builder.ComplexProperty(v => v.PhoneNumber, pn =>
        {
            pn.Property(p => p.RegionCode)
               .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
               .HasColumnName("phone_region_code");

            pn.Property(p => p.Number)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                .HasColumnName("phone_number");
        });

        builder.Property(v => v.Requisites)
            .HasConversion(new ReadOnlyListConverter<RequisitesInfo>())
            .Metadata.SetValueComparer(new ReadOnlyListComparer<RequisitesInfo>());

        builder.Property(v => v.SocialNetworks)
           .HasConversion(new ReadOnlyListConverter<SocialNetworkInfo>())
           .Metadata.SetValueComparer(new ReadOnlyListComparer<SocialNetworkInfo>());

        //For soft deleting
        builder.Property<bool>("_isDeleted")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("is_deleted");

        builder.Property<DateTime?>("_deletedDateTime")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("deleted_date_time");

        // Relationships
        builder.HasMany(v => v.Pets)
            .WithOne()
            .HasForeignKey("volunteer_id") 
            .OnDelete(DeleteBehavior.Cascade);
    }
}

