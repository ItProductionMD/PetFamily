

using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Queries.Volunteer.GetVolunteer;

public record GetVolunteerQuery(Guid Id) : IQuery;

