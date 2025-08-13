using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Discussions.Application.Queries.GetDiscussion;

public record GetDiscussionQuery(
    Guid UserId, 
    Guid DiscussionId,
    int Page,
    int PageSize) : IQuery;

