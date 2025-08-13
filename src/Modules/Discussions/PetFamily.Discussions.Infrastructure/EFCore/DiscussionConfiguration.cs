using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.SharedInfrastructure.Shared.EFCore;
using PetFamily.SharedKernel.ValueObjects;
using static PetFamily.SharedInfrastructure.Shared.EFCore.Convertors;

namespace PetFamily.Discussions.Infrastructure.EFCore;

public class DiscussionConfiguration : IEntityTypeConfiguration<Discussion>
{
    public void Configure(EntityTypeBuilder<Discussion> builder)
    {
        builder.ToTable("discussions");

        builder.HasKey(d => d.Id);

        builder.Property(d=>d.RelationId)
            .HasColumnName("relation_id")
            .IsRequired();

        builder.Property(d => d.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(d => d.IsClosed)
            .HasColumnName("is_closed");

        builder.Property(d=>d.ParticipantIds)
            .HasColumnName("participant_ids")
            .HasColumnType("jsonb")
            .HasConversion(new ReadOnlyListConverter<Guid>())
            .Metadata.SetValueComparer(new ReadOnlyListComparer<Guid>());

        //relationships
        builder.HasMany(d=>d.Messages)
            .WithOne()
            .HasForeignKey(m => m.DiscussionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
