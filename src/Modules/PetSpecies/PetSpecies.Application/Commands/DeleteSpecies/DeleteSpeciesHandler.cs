using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetSpecies.Application.IRepositories;
using Volunteers.Public.IContracts;

namespace PetSpecies.Application.Commands.DeleteSpecies;

public class DeleteSpeciesHandler(
    IPetExistenceContract petExistenceChecker,
    ISpeciesWriteRepository speciesWriteRepo,
    ILogger<DeleteSpeciesHandler> logger) : ICommandHandler<DeleteSpeciesCommand>
{
    public async Task<UnitResult> Handle(DeleteSpeciesCommand cmd, CancellationToken ct)
    {
        if (cmd.SpeciesId == Guid.Empty)
        {
            logger.LogWarning("SpeciesId cannot be empty");
            return UnitResult.Fail(Error.GuidIsEmpty("SpeciesId"));
        }

        var isPetExists = await petExistenceChecker.ExistsWithSpeciesAsync(cmd.SpeciesId, ct);
        if (isPetExists)
            return UnitResult.Fail(Error.Conflict("Cant delete species because it is used by a pet"));

        var deleteResult = await speciesWriteRepo.DeleteAndSaveAsync(cmd.SpeciesId, ct);

        return deleteResult;
    }
}
