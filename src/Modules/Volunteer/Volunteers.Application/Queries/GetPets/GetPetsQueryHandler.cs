using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Queries.GetPets;

public class GetPetsQueryHandler(
    IVolunteerReadRepository repository)
    : IQueryHandler<GetPetsResponse, GetPetsQuery>
{
    private readonly IVolunteerReadRepository _repository = repository;

    public async Task<Result<GetPetsResponse>> Handle(GetPetsQuery query, CancellationToken ct)
    {
        var validationResult = GetPetsQueryValidator.Validate(query);
        if (validationResult.IsFailure)
            return validationResult;

        return await _repository.GetPetPagedList(
            query.PetsFilter,
            query.PageNumber,
            query.PageSize,
            ct);
    }
}
