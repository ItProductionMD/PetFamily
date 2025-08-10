using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetSpecies.Application.Commands.AddSpecies;
using PetSpecies.Application.IRepositories;
using PetSpecies.Domain;


namespace PetFamily.SharedApplication.Commands.PetTypeManagement.AddPetType;

public class AddPetTypeHandler(
    ISpeciesWriteRepository repository,
    ILogger<AddPetTypeHandler> logger) : ICommandHandler<Species, AddPetTypeComand>
{
    private ILogger<AddPetTypeHandler> _logger = logger;
    private readonly ISpeciesWriteRepository _repository = repository;
    public async Task<Result<Species>> Handle(
        AddPetTypeComand command,
        CancellationToken token)
    {
        var validationResult = AddPetTypeCommandValidator.Validate(command);
        if (validationResult.IsFailure)
        {
            _logger.LogWarning("Validate pet type errors: {Errors}",
                validationResult.ValidationMessagesToString());

            return validationResult;
        }
        var species = Species.Create(SpeciesID.NewGuid(), command.SpeciesName).Data!;

        var breeds = command.BreedList
            .Select(b => Breed.Create(BreedID.NewGuid(), b.Name, b.Description).Data!).ToList();

        species.AddBreeds(breeds);

        await _repository.AddAsync(species, token);

        _logger.LogInformation("PetType with id:{speciesId} added succesfully!", species.Id);

        return Result.Ok(species);
    }
}
