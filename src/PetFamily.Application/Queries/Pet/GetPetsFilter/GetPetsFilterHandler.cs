using PetFamily.Application.Abstractions;
using PetFamily.Application.Dtos;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Queries.Pet.GetPetsFilter;

public class GetPetsFilterHandler(
    ISpeciesReadRepository speciesReadRepository)
    : IQueryHandler<GetPetsFilterDto,GetPetsFilterQuery>
{
    private readonly ISpeciesReadRepository _speciesReadRepository = speciesReadRepository;

    public async Task<Result<GetPetsFilterDto>> Handle(
        GetPetsFilterQuery query,
        CancellationToken cancellationToken = default)
    {
        //TODO ADD REDIS AND CHEKING VERSIONED CACHE 
        if (query.HasFilter == true)
            return Result.Ok(new GetPetsFilterDto());

        var resultForSpecies = await _speciesReadRepository.GetSpeciesDtos(cancellationToken);

        return resultForSpecies.IsFailure
            ? Result.Fail(resultForSpecies.Error)
            : Result.Ok(new GetPetsFilterDto(resultForSpecies.Data));
    }
}
