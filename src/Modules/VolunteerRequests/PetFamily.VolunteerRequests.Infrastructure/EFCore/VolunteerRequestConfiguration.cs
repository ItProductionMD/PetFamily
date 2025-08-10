using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.SharedInfrastructure.Shared.EFCore;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.VolunteerRequests.Domain.Entities;
using static PetFamily.SharedInfrastructure.Shared.EFCore.Convertors;
using static PetFamily.SharedKernel.Validations.ValidationConstants;

namespace PetFamily.VolunteerRequests.Infrastructure.EFCore;

public class VolunteerRequestConfiguration : IEntityTypeConfiguration<VolunteerRequest>
{
    public void Configure(EntityTypeBuilder<VolunteerRequest> builder)
    {
        builder.ToTable("volunteer_requests");

        builder.HasKey(vr => vr.Id);

        builder.Property(vr => vr.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(v => v.AdminId)
            .HasColumnName("admin_id");

        builder.Property(v => v.DiscussionId)
            .HasColumnName("discussion_id");

        builder.Property(vr => vr.DocumentName)
            .HasColumnName("document_name")
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
            .IsRequired();

        builder.Property(vr => vr.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
            .IsRequired();

        builder.Property(vr => vr.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(MAX_LENGTH_SHORT_TEXT)
            .IsRequired();

        builder.Property(vr => vr.Description)
            .HasColumnName("description")
            .HasMaxLength(MAX_LENGTH_MEDIUM_TEXT)
            .IsRequired(false);

        builder.Property(vr => vr.ExperienceYears)
            .HasColumnName("experience_years")
            .IsRequired();

        builder.Property(vr => vr.RequestStatus)
            .HasColumnName("request_status")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(vr => vr.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(vr => vr.RejectedAt)
           .HasColumnName("rejected_at");

        builder.Property(v => v.RejectedComment)
            .HasColumnName("rejected_comment")
            .HasMaxLength(MAX_LENGTH_LONG_TEXT);

        builder.Property(v => v.Requisites)
            .HasColumnName("requisites")
            .HasColumnType("jsonb")
            .HasConversion(new ReadOnlyListConverter<RequisitesInfo>())
            .Metadata.SetValueComparer(new ReadOnlyListComparer<RequisitesInfo>());
    }
}
