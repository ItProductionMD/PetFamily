using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Volunteers.Application.Queries.GetVolunteer;

public record GetVolunteerQuery(Guid Id) : IQuery;

