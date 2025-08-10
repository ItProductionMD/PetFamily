using PetFamily.SharedInfrastructure.Constants;

namespace PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;

public static class DiscussionsTable
{
    public const string TableName = "discussions";
    public const string TableFullName = "discussion.discussions";
    public const string Id = "id";
    public const string RelationId = "relation_id";
    public const string IsClosed = "is_closed";
    public const string ParticipantIds = "participant_ids";
    public const string IsDeleted = "is_deleted";
    public const string DeletedAt = "deleted_at";
}
