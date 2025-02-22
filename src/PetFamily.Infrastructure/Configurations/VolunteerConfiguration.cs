using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using System.Reflection.Emit;
using System.Text.Json;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;

namespace PetFamily.Infrastructure.Configurations
{
    public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
    {
        public void Configure(EntityTypeBuilder<Volunteer> builder)
        {
            builder.ToTable("Volunteers");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id);

            builder.Property(v => v.Email)
                .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT)
                .IsRequired();

            builder.Property(v => v.ExperienceYears);

            builder.Property(v => v.Description)
                .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT);

            // Value Objects
            builder.OwnsOne(v => v.FullName, fn =>
            {
                fn.Property(f => f.FirstName)
                    .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                    .IsRequired();

                fn.Property(f => f.LastName)
                    .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                    .IsRequired();
            });

            builder.OwnsOne(v => v.PhoneNumber, pn =>
            {
                pn.Property(p => p.RegionCode)
                   .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                   .IsRequired();

                pn.Property(p => p.Number)
                    .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                    .IsRequired();
            });

            builder.Property(v => v.Requisites)
                .HasConversion(
                    v => JsonSerializer.Serialize(v,JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<IReadOnlyList<RequisitesInfo>>(v,JsonSerializerOptions.Default)
                    ?? new List<RequisitesInfo>())

                .Metadata.SetValueComparer(
                    new ValueComparer<IReadOnlyList<RequisitesInfo>>(
                    (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && c1.SequenceEqual(c2),
                    c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
                    c => c != null ? c.ToList() : new List<RequisitesInfo>())
                );

            builder.Property(v => v.SocialNetworks)
               .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<IReadOnlyList<SocialNetworkInfo>>(v, JsonSerializerOptions.Default)
                    ?? new List<SocialNetworkInfo>())

                .Metadata.SetValueComparer(
                    new ValueComparer<IReadOnlyList<SocialNetworkInfo>>(
                    (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && c1.SequenceEqual(c2),
                    c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
                    c => c != null ? c.ToList() : new List<SocialNetworkInfo>())
                );

            //For soft deleting
            builder.Property<bool>("_isDeleted")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("is_deleted");

            builder.Property<DateTime?>("_deletedDateTime")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("deleted_date_time");

            // Relationships
            builder.HasMany(v => v.Pets)
                .WithOne(p=>p.Volunteer)
                .HasForeignKey("volunteer_id") 
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

