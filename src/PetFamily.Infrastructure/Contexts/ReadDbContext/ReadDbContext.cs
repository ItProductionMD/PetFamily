using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetFamily.Infrastructure.Contexts.ReadDbContext.Models;

namespace PetFamily.Infrastructure.Contexts.ReadDbContext;

public partial class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Breed> Breeds { get; set; }

    public virtual DbSet<Pet> Pets { get; set; }

    public virtual DbSet<Species> Species { get; set; }

    public virtual DbSet<Volunteer> Volunteers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Breed>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_breeds");

            entity.ToTable("breeds");

            entity.HasIndex(e => e.SpeciesId, "ix_breeds_species_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.SpeciesId).HasColumnName("species_id");

            entity.HasOne(d => d.Species).WithMany(p => p.Breeds)
                .HasForeignKey(d => d.SpeciesId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_breeds_animal_types_species_id");
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_pets");

            entity.ToTable("pets");

            entity.HasIndex(e => e.Name, "ix_pets_name");

            entity.HasIndex(e => e.VolunteerId, "ix_pets_volunteer_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AddressCity)
                .HasMaxLength(50)
                .HasColumnName("address_city");
            entity.Property(e => e.AddressRegion)
                .HasMaxLength(50)
                .HasColumnName("address_region");
            entity.Property(e => e.AddressStreet)
                .HasMaxLength(500)
                .HasColumnName("address_street");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.DateTimeCreated).HasColumnName("date_time_created");
            entity.Property(e => e.DeletedDateTime).HasColumnName("deleted_date_time");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.HealthInfo).HasColumnName("health_info");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.HelpStatus).HasColumnName("help_status");
            entity.Property(e => e.Images).HasColumnName("images");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsSterilized).HasColumnName("is_sterilized");
            entity.Property(e => e.IsVaccinated).HasColumnName("is_vaccinated");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.OwnerPhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("owner_phone_number");
            entity.Property(e => e.OwnerPhoneRegionCode)
                .HasMaxLength(50)
                .HasColumnName("owner_phone_region_code");
            entity.Property(e => e.PetTypeBreedId).HasColumnName("pet_type_breed_id");
            entity.Property(e => e.PetTypeSpeciesId).HasColumnName("pet_type_species_id");
            entity.Property(e => e.Requisites).HasColumnName("requisites");
            entity.Property(e => e.SerialNumber).HasColumnName("serial_number");
            entity.Property(e => e.VolunteerId).HasColumnName("volunteer_id");
            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.Pets)
                .HasForeignKey(d => d.VolunteerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_pets_volunteers_volunteer_id");
        });

        modelBuilder.Entity<Species>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_species");

            entity.ToTable("species");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Volunteer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_volunteers");

            entity.ToTable("volunteers");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DeletedDateTime).HasColumnName("deleted_date_time");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(500)
                .HasColumnName("email");
            entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.PhoneRegionCode)
                .HasMaxLength(50)
                .HasColumnName("phone_region_code");
            entity.Property(e => e.Rating)
                .HasDefaultValue(0)
                .HasColumnName("rating");
            entity.Property(e => e.Requisites).HasColumnName("requisites");
            entity.Property(e => e.SocialNetworks).HasColumnName("social_networks");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
