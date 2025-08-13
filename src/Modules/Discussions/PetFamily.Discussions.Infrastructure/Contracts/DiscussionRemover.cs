using PetFamily.Discussions.Public.Contracts;

namespace PetFamily.Discussions.Infrastructure.Contracts;

public class DiscussionRemover : IDiscussionRemover
{
    public async Task RemoveDisscusion(Guid discussionId)
    {
        // Here you would implement the logic to remove the discussion.
        // For now, we will just simulate the operation with a delay.
        await Task.Delay(100); // Simulating async operation
        // If the operation is successful, you might return a success result.
        // If there is an error, you would throw an exception or return an error result.
    }
}
