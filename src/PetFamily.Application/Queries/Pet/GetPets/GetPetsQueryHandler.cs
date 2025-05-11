using PetFamily.Application.Abstractions;
using PetFamily.Domain.Results;
using PetFamily.Application.IRepositories;

namespace PetFamily.Application.Queries.Pet.GetPets;

public class GetPetsQueryHandler(
    IVolunteerReadRepository volunteerReadRepository) : IQueryHandler<GetPetsResponse, GetPetsQuery>
{
    private readonly IVolunteerReadRepository _volunteerReadRepository = volunteerReadRepository;

    public async Task<Result<GetPetsResponse>> Handle(GetPetsQuery query, CancellationToken token)
    {
        // Validate query
        if (query.PageNumber <= 0)
            throw new ArgumentException("Page number must be greater than 0");
        if (query.PageSize <= 0)
            throw new ArgumentException("Page size must be greater than 0");

        var result = await _volunteerReadRepository.GetPets(
            query.PetsFilter,
            query.PageNumber,
            query.PageSize,
            token);

        return result;
    }
}
