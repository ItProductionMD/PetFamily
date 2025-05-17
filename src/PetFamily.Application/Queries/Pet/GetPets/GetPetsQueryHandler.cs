using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Queries.Pet.GetPets;

public class GetPetsQueryHandler(
    IVolunteerReadRepository volunteerReadRepository) : IQueryHandler<GetPetsResponse, GetPetsQuery>
{
    private readonly IVolunteerReadRepository _volunteerReadRepository = volunteerReadRepository;

    public async Task<Result<GetPetsResponse>> Handle(GetPetsQuery query, CancellationToken token)
    {
        var validationResult = GetPetsQueryValidator.Validate(query);
        if (validationResult.IsFailure)
            return validationResult;

        return await _volunteerReadRepository.GetPets(
            query.PetsFilter,
            query.PageNumber,
            query.PageSize,
            token);
    }
}
