using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetSpecies.Application.IRepositories;
using Volunteers.Public.IContracts;

namespace PetSpecies.Application.Commands.DeleteBreed;

public class DeleteBreedHandler(
    IPetExistenceContract petExistenceChecker,
    ISpeciesWriteRepository speciesWriteRepository,
    ILogger<DeleteBreedHandler> logger) : ICommandHandler<DeleteBreedCommand>
{
    private readonly ILogger<DeleteBreedHandler> _logger = logger;
    private readonly IPetExistenceContract _petExistenceChecker = petExistenceChecker;
    private readonly ISpeciesWriteRepository _writeRepository = speciesWriteRepository;

    public async Task<UnitResult> Handle(DeleteBreedCommand command, CancellationToken cancelToken)
    {
        if (command.BreedId == Guid.Empty)
        {
            _logger.LogWarning("BreedId cannot be empty");
            return UnitResult.Fail(Error.GuidIsEmpty("BreedId"));
        }
        var isPetExist = await _petExistenceChecker.ExistsWithBreedAsync(command.BreedId);
        if (isPetExist)
            return UnitResult.Fail(Error.Conflict("Breed can't be deleted because it is used by a pet!"));

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
