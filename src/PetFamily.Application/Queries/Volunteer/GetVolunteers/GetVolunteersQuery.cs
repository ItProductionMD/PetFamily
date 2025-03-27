using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Queries.Volunteer.GetVolunteers;

public record GetVolunteersQuery(
    int pageSize,
    int pageNumber,
    string? orderBy,
    string? orderDirection) : IQuery;

