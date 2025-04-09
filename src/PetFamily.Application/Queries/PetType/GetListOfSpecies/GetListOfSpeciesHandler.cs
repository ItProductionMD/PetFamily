
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;
using System.Numerics;

namespace PetFamily.Application.Queries.PetType.GetListOfSpecies;

public class GetListOfSpeciesHandler(
    ISpeciesReadRepository readRepository,
    ILogger<GetListOfSpeciesHandler> logger)
    : IQueryHandler<GetListOfSpeciesResponse, GetListOfSpeciesQuery>
{
    private readonly ISpeciesReadRepository _readRepository = readRepository;
    private readonly ILogger<GetListOfSpeciesHandler> _logger = logger;

    public async Task<Result<GetListOfSpeciesResponse>> Handle(
        GetListOfSpeciesQuery query,
        CancellationToken cancelToken)
    {
        var response = await _readRepository.GetListOfSpeciesAsync(query, cancelToken);

        return Result.Ok(response);
    }
}
