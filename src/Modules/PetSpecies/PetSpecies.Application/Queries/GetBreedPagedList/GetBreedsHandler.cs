using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetSpecies.Application.IRepositories;

namespace PetSpecies.Application.Queries.GetBreedPagedList;

public class GetBreedsHandler(ISpeciesReadRepository speciesReadRepo)
    : IQueryHandler<BreedPagedListDto, GetBreedPageListQuery>
{
    public async Task<Result<BreedPagedListDto>> Handle(
        GetBreedPageListQuery query,
        CancellationToken ct)
    {
        //TODO Validate query
        BreedFilterDto filterDto = new();
        var response = await speciesReadRepo.GetBreedPagedList(
            query.BreedFilter,
            query.Pagination,
            ct);

        return Result.Ok(response);
    }
}
