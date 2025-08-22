using PetFamily.Discussions.Domain.Entities;

namespace PetFamily.IntegrationTests.DiscussionFeatures;

public class DiscussionTestDataCreator
{
    public Guid RequestId { get; set; } = Guid.NewGuid();
    public Guid AdminId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = Guid.NewGuid();
    public Discussion CreateDiscussion()
    {
        var discussion = Discussion.Create(RequestId, AdminId, UserId).Data!;

        return discussion;
    }
}
