using PetFamily.SharedInfrastructure.Constants;

namespace PetFamily.SharedInfrastructure.Shared.Dapper.ScaffoldedClassesPreview;

public static class MessagesTable
{
    public const string TableName = "messages";
    public const string TableFullName = "discussion.messages";
    public const string Id = "id";
    public const string DiscussionId = "discussion_id";
    public const string AuthorId = "author_id";
    public const string Text = "text";
    public const string CreatedAt = "created_at";
    public const string EditedAt = "edited_at";
}
