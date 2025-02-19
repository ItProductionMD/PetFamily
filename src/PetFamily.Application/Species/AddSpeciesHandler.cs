using Microsoft.Extensions.Logging;
using PetFamily.Domain.PetAggregates.Entities;
using PetFamily.Domain.PetAggregates.ValueObjects;
using PetFamily.Domain.Results;
using PetSpecies = PetFamily.Domain.PetAggregates.Entities.Species;


namespace PetFamily.Application.Species;

public class AddSpeciesHandler(ISpeciesRepository repository, ILogger<AddSpeciesHandler> logger)
{
    private ILogger<AddSpeciesHandler> _logger = logger;
    private readonly ISpeciesRepository _repository= repository;
    public async Task<Result<PetSpecies>> Handle(
        AddPetTypeRequest request,
        CancellationToken token)
    {
        var validationResult = AddSpeciesRequestValidator.Validate(request);
        if (validationResult.IsFailure)
        {
            _logger.LogWarning("Validate pet type errors: {Errors}",
                string.Join("; ",validationResult.Errors.Select(e=>e.Message)));
            return validationResult;
        }         
        var species = PetSpecies.Create(SpeciesID.NewGuid(), request.SpeciesName).Data!;

        var breeds = request.BreedList
            .Select(b => Breed.Create(BreedID.NewGuid(),b.Name, b.Description).Data!).ToList();

        species.AddBreeds(breeds);

        await _repository.AddAsync(species,token);

        _logger.LogInformation("PetType with id:{speciesId} added succesfully!",species.Id);

        return Result.Ok(species);
    }
}
