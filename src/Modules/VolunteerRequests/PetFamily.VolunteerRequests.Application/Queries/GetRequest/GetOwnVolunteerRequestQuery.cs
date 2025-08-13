using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.VolunteerRequests.Application.Queries.GetRequest;

public record GetOwnVolunteerRequestQuery(Guid UserId) : IQuery;
