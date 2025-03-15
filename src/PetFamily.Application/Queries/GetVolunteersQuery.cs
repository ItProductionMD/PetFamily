
namespace PetFamily.Application.Queries;

public record GetVolunteersQuery(int pageSize, int pageNumber, string? orderBy, string? orderDirection);

