using PetFamily.Application.Abstractions.CQRS;

namespace Volunteers.Application.Queries.GetVolunteers;

public record GetVolunteersQuery(
    int pageSize,
    int pageNumber,
    string? orderBy,
    string? orderDirection) : IQuery;


