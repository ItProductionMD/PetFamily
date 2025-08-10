using PetFamily.Application.Abstractions.CQRS;

namespace PetFamily.VolunteerRequests.Application.Queries.GetRequestsOnReview;

public record GetRequestsOnReviewQuery(
    int Page, 
    int PageSize,
    VolunteerRequestsFilter Filter) : IQuery;
