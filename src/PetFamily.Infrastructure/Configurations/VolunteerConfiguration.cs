using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.VolunteerAggregates.Root;
using PetFamily.Domain.Shared;
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

            builder.Property(v => v.ExpirienceYears);

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
            
            builder.OwnsOne(v => v.DonateDetailsList,d =>
            {
                d.ToJson();
                d.OwnsMany(d => d.ObjectList, db =>
                {
                    db.Property(d => d.Name)
                        .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                        .IsRequired();

                    db.Property(d => d.Description)
                        .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT)
                        .IsRequired();
                });
            });

            builder.OwnsOne(v => v.SocialNetworksList, d =>
            {
                d.ToJson();
                d.OwnsMany(d => d.ObjectList,db =>
                {
                    db.Property(d => d.Name)
                        .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                        .IsRequired();

                    db.Property(d=>d.Url)
                        .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT)
                        .IsRequired();
                });
            });
            // Relationships
            builder.HasMany(v => v.Pets)
                .WithOne(p => p.Volunteer)
                .HasForeignKey("VolunteerId") // Define the foreign key on Pet
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            builder.Navigation(v => v.Pets)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}

