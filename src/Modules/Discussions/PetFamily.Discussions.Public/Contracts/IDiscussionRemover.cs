namespace PetFamily.Discussions.Public.Contracts
{
    public interface IDiscussionRemover
    {
        Task RemoveDisscusion(Guid discussionId);
    }
}
