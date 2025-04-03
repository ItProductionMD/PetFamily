using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetTypeManagment.Entities;
using PetFamily.Domain.Results;
using PetSpecies = PetFamily.Domain.PetTypeManagment.Root.Species;


namespace PetFamily.Application.Commands.PetTypeManagment.AddPetType;

public class AddPetTypeHandler(
    ISpeciesWriteRepository repository,
    ILogger<AddPetTypeHandler> logger) : ICommandHandler<PetSpecies, AddPetTypeComand>
{
    private ILogger<AddPetTypeHandler> _logger = logger;
    private readonly ISpeciesWriteRepository _repository = repository;
    public async Task<Result<PetSpecies>> Handle(
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
        var species = PetSpecies.Create(SpeciesID.NewGuid(), command.SpeciesName).Data!;

        var breeds = command.BreedList
            .Select(b => Breed.Create(BreedID.NewGuid(), b.Name, b.Description).Data!).ToList();

        species.AddBreeds(breeds);

        await _repository.AddAsync(species, token);

        _logger.LogInformation("PetType with id:{speciesId} added succesfully!", species.Id);

        return Result.Ok(species);
    }
}
