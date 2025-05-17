using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Dtos;
using PetFamily.Application.IRepositories;
using PetFamily.Application.Queries.Pet.GetPets;
using PetFamily.Application.Queries.Volunteer.GetVolunteers;
using PetFamily.Domain.Results;
using PetFamily.Infrastructure.Contexts.ReadDbContext;

namespace PetFamily.Infrastructure.Repositories.Read;

public class VolunteerReadRepository(ReadDbContext context) : IVolunteerReadRepository
{
    private readonly ReadDbContext _context = context;

    public Task<Result<VolunteerDto>> GetByIdAsync(Guid volunteerId, CancellationToken cancellToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<GetVolunteersResponse> GetVolunteers(
        GetVolunteersQuery query,
        CancellationToken cancelToken = default)
    {
        var volunteersQuery = _context.Volunteers.AsNoTracking();

        var filteredQuery = query.orderBy switch
        {
            "full_name" => query.orderDirection?.ToLower() == "desc"
                ? volunteersQuery.OrderByDescending(v => v.LastName).ThenByDescending(v => v.FirstName)
                : volunteersQuery.OrderBy(v => v.LastName).ThenBy(v => v.FirstName),

            "rating" => query.orderDirection?.ToLower() == "desc"
                ? volunteersQuery.OrderByDescending(v => v.Rating)
                : volunteersQuery.OrderBy(v => v.Rating),

            _ => query.orderDirection?.ToLower() == "desc"
                ? volunteersQuery.OrderByDescending(v => v.Id)
                : volunteersQuery.OrderBy(v => v.Id)
        };
        var volunteersList = await filteredQuery
            .Skip((query.pageNumber - 1) * query.pageSize)
            .Take(query.pageSize + 1)
            .Select(v => new VolunteerMainInfoDto(v.Id, v.FullName, v.Phone, v.Rating))
            .ToListAsync(cancelToken);

        bool hasMoreRecords = volunteersList.Count > query.pageSize;

        if (hasMoreRecords)
            volunteersList.RemoveAt(volunteersList.Count - 1);

        var totalCount = hasMoreRecords
            ? await volunteersQuery.CountAsync(cancelToken)
            : (query.pageNumber - 1) * query.pageSize + volunteersList.Count;

        return new(totalCount, volunteersList);
    }

    public Task<UnitResult> CheckUniqueFields(
        Guid volunteerId,
        string phoneRegionCode,
        string phoneNuber,
        string email,
        CancellationToken cancelToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<GetPetsResponse>> GetPets(PetsFilter filter, int pageNumber, int pageSize, CancellationToken cancelToken = default)
    {
        throw new NotImplementedException();
    }
}
