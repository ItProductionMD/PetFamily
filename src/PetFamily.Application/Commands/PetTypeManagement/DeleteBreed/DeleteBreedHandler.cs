using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.PetTypeManagement.DeleteBreed;

public class DeleteBreedHandler(
    ISpeciesReadRepository speciesReadRepository,
    ISpeciesWriteRepository speciesWriteRepository,
    ILogger<DeleteBreedHandler> logger) : ICommandHandler<DeleteBreedCommand>
{
    private readonly ILogger<DeleteBreedHandler> _logger = logger;
    private readonly ISpeciesReadRepository _readRepository = speciesReadRepository;
    private readonly ISpeciesWriteRepository _writeRepository = speciesWriteRepository;

    public async Task<UnitResult> Handle(DeleteBreedCommand command, CancellationToken cancelToken)
    {
        if (command.BreedId == Guid.Empty)
        {
            _logger.LogWarning("BreedId cannot be empty");
            return UnitResult.Fail(Error.GuidIsEmpty("BreedId"));
        }
        var isPermitedToDelete = await _readRepository.CheckForDeleteBreedAsync(command.BreedId);
        if (isPermitedToDelete.IsFailure)
            return isPermitedToDelete;

        var getSpecies = await _writeRepository.GetByBreedIdAsync(command.BreedId, cancelToken);
        if (getSpecies.IsFailure)
            return UnitResult.Fail(getSpecies.Error);

        var species = getSpecies.Data!;

        var deleteResult = species.DeleteBreedById(command.BreedId);
        if (deleteResult.IsFailure)
            return UnitResult.Fail(deleteResult.Error);

        await _writeRepository.SaveAsync(species, cancelToken);

        return UnitResult.Ok();
    }
}
