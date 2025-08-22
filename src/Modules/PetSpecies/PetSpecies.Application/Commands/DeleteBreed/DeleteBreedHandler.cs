using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetSpecies.Application.IRepositories;
using Volunteers.Public.IContracts;

namespace PetSpecies.Application.Commands.DeleteBreed;

public class DeleteBreedHandler(
    IPetExistenceContract petExistenceChecker,
    ISpeciesWriteRepository speciesWriteRepo,
    ILogger<DeleteBreedHandler> logger) : ICommandHandler<DeleteBreedCommand>
{
    public async Task<UnitResult> Handle(DeleteBreedCommand cmd, CancellationToken ct)
    {
        if (cmd.BreedId == Guid.Empty)
        {
            logger.LogWarning("BreedId cannot be empty");
            return UnitResult.Fail(Error.GuidIsEmpty("BreedId"));
        }
        var isPetExist = await petExistenceChecker.ExistsWithBreedAsync(cmd.BreedId);
        if (isPetExist)
            return UnitResult.Fail(Error.Conflict("Breed can't be deleted because it is used by a pet!"));

        var getSpecies = await speciesWriteRepo.GetByBreedIdAsync(cmd.BreedId, ct);
        if (getSpecies.IsFailure)
            return UnitResult.Fail(getSpecies.Error);

        var species = getSpecies.Data!;

        var deleteResult = species.DeleteBreedById(cmd.BreedId);
        if (deleteResult.IsFailure)
            return UnitResult.Fail(deleteResult.Error);

        await speciesWriteRepo.SaveAsync(species, ct);

        return UnitResult.Ok();
    }
}
