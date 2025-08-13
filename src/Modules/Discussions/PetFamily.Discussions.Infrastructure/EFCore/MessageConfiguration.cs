using PetFamily.SharedKernel.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Discussions.Domain.Entities;

namespace PetFamily.Discussions.Infrastructure.EFCore;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id");

        builder.Property(m => m.DiscussionId)
            .HasColumnName("discussion_id")
            .IsRequired();

        builder.Property(m => m.AuthorId)
            .HasColumnName("author_id")
            .IsRequired();

        builder.Property(m => m.Text)
            .HasColumnName("text")
            .HasMaxLength(ValidationConstants.MAX_LENGTH_LONG_TEXT)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(m => m.EditedAt)
            .HasColumnName("edited_at");

        // Optional: Indexes
        builder.HasIndex(m => m.DiscussionId);
        builder.HasIndex(m => m.AuthorId);
    }
}

