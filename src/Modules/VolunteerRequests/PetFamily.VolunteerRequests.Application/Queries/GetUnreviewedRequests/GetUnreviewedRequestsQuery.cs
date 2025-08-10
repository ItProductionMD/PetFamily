using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.VolunteerRequests.Application.Queries.GetUnreviewedRequests;

public sealed record GetUnreviewedRequestsQuery(
    int Page,
    int PageSize
) : IQuery;
