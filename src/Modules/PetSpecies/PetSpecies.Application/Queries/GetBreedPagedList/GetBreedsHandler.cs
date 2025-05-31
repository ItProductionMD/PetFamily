using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetSpecies.Application.IRepositories;

namespace PetSpecies.Application.Queries.GetBreedPagedList;

public class GetBreedsHandler(
    ISpeciesReadRepository speciesReadRepository,
    ILogger<GetBreedsHandler> logger) : IQueryHandler<BreedPagedListDto, GetBreedPageListQuery>
{
    private readonly ISpeciesReadRepository _readRepository = speciesReadRepository;
    private readonly ILogger<GetBreedsHandler> _logger = logger;

    public async Task<Result<BreedPagedListDto>> Handle(
        GetBreedPageListQuery query,
        CancellationToken cancelToken)
    {
        //Validate query
        BreedFilterDto filterDto = new();
        var response = await _readRepository.GetBreedPagedList(
            query.BreedFilter,
            query.Pagination,
            cancelToken);

        return Result.Ok(response);
    }
}
