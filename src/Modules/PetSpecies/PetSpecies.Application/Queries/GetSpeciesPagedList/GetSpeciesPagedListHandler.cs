using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetSpecies.Application.IRepositories;

namespace PetSpecies.Application.Queries.GetSpeciesPagedList;

public class GetSpeciesPagedListHandler(
    ISpeciesReadRepository readRepository,
    ILogger<GetSpeciesPagedListHandler> logger)
    : IQueryHandler<SpeciesPagedListDto, GetSpeciesPagedListQuery>
{
    private readonly ISpeciesReadRepository _readRepository = readRepository;
    private readonly ILogger<GetSpeciesPagedListHandler> _logger = logger;

    public async Task<Result<SpeciesPagedListDto>> Handle(
        GetSpeciesPagedListQuery query,
        CancellationToken ct)
    {
        //TODO Validate query

        var response = await _readRepository.GetSpeciesPagedList(
            query.SpeciesFilter,
            query.Pagination,
            ct);

        return Result.Ok(response);
    }
}
