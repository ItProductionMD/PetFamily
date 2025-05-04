
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.PetTypeManagement.DeleteSpecies;

public class DeleteSpeciesHandler(
    ISpeciesReadRepository speciesReadRepository,
    ISpeciesWriteRepository speciesWriteRepository,
    ILogger<DeleteSpeciesHandler> logger) : ICommandHandler<DeleteSpeciesCommand>
{
    private readonly ILogger<DeleteSpeciesHandler> _logger = logger;
    private readonly ISpeciesReadRepository _speciesReadRepository = speciesReadRepository;
    private readonly ISpeciesWriteRepository _speciesWriteRepository = speciesWriteRepository;

    public async Task<UnitResult> Handle(DeleteSpeciesCommand command, CancellationToken cancelToken)
    {
        if (command.SpeciesId == Guid.Empty)
        {
            _logger.LogWarning("SpeciesId cannot be empty");
            return UnitResult.Fail(Error.GuidIsEmpty("SpeciesId"));
        }

        var isPermitedToDelete = await _speciesReadRepository.CheckForDeleteAsync(command.SpeciesId);
        if (isPermitedToDelete.IsFailure)
            return isPermitedToDelete;

        var deleteResult = await _speciesWriteRepository.DeleteAsync(command.SpeciesId, cancelToken);

        return deleteResult;
    }
}
