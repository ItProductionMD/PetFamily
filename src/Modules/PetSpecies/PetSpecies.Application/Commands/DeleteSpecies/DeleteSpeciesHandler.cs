using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetSpecies.Application.IRepositories;
using Volunteers.Public.IContracts;

namespace PetSpecies.Application.Commands.DeleteSpecies;

public class DeleteSpeciesHandler(
    IPetExistenceContract petExistenceChecker,
    ISpeciesWriteRepository speciesWriteRepository,
    ILogger<DeleteSpeciesHandler> logger) : ICommandHandler<DeleteSpeciesCommand>
{
    private readonly ILogger<DeleteSpeciesHandler> _logger = logger;
    private readonly IPetExistenceContract _petExistenceChecker = petExistenceChecker;
    private readonly ISpeciesWriteRepository _speciesWriteRepository = speciesWriteRepository;

    public async Task<UnitResult> Handle(DeleteSpeciesCommand command, CancellationToken cancelToken)
    {
        if (command.SpeciesId == Guid.Empty)
        {
            _logger.LogWarning("SpeciesId cannot be empty");
            return UnitResult.Fail(Error.GuidIsEmpty("SpeciesId"));
        }

        var isPetExists = await _petExistenceChecker.ExistsWithSpeciesAsync(command.SpeciesId);
        if (isPetExists)
            return UnitResult.Fail(Error.Conflict("Cant delete species because it is used by a pet"));

        var deleteResult = await _speciesWriteRepository.DeleteAsync(command.SpeciesId, cancelToken);

        return deleteResult;
    }
}
