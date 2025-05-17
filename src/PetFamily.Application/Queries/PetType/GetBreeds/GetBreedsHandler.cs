using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Queries.PetType.GetBreeds;

public class GetBreedsHandler(
    ISpeciesReadRepository speciesReadRepository,
    ILogger<GetBreedsHandler> logger) : IQueryHandler<GetBreedsResponse, GetBreedsQuery>
{
    private readonly ISpeciesReadRepository _readRepository = speciesReadRepository;
    private readonly ILogger<GetBreedsHandler> _logger = logger;

    public async Task<Result<GetBreedsResponse>> Handle(
        GetBreedsQuery query,
        CancellationToken cancelToken)
    {
        if (query.SpeciesId == Guid.Empty)
        {
            _logger.LogWarning("SpeciesId cannot be empty");
            return Result.Fail(Error.GuidIsEmpty("SpeciesId"));
        }
        var response = await _readRepository.GetBreedsAsync(query, cancelToken);
        return Result.Ok(response);
    }
}
