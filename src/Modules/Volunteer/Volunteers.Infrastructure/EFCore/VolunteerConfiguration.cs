using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.SharedInfrastructure.Shared.EFCore;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using static PetFamily.SharedInfrastructure.Shared.EFCore.Convertors;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using VolunteerDomain = Volunteers.Domain.Volunteer;

namespace Volunteers.Infrastructure.EFCore;

public class VolunteerConfiguration : IEntityTypeConfiguration<VolunteerDomain>
{
    public void Configure(EntityTypeBuilder<VolunteerDomain> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id);

        builder.Property(v => v.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value).Data!)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(v => v.ExperienceYears);

        builder.Property(v => v.Description)
            .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT);

        // Value Objects
        builder.ComplexProperty(v => v.FullName, fn =>
        {
            fn.Property(f => f.FirstName)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                .HasColumnName("first_name");

            fn.Property(f => f.LastName)
                .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
                .HasColumnName("last_name");
        });

        builder.Property(v => v.Phone)
            .HasColumnName("phone")
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
            .IsRequired();

        builder.Property(v => v.Requisites)
            .HasColumnType("jsonb")
            .HasConversion(new ReadOnlyListConverter<RequisitesInfo>())
            .Metadata.SetValueComparer(new ReadOnlyListComparer<RequisitesInfo>());

        //For soft deleting
        builder.Property(v => v.IsDeleted)
            .HasColumnName("is_deleted");

        builder.Property(v => v.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        

        // Relationships
        builder.HasMany(v => v.Pets)
            .WithOne()
            .HasForeignKey("volunteer_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.ApplyUniqueConstraints();
    }
}

