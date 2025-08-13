using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetSpecies.Application.Commands.AddSpecies;
using PetSpecies.Application.Commands.CommandsDtos;
using PetSpecies.Application.IRepositories;
using PetSpecies.Domain;
using PetSpecies.Public.Dtos;


namespace PetFamily.SharedApplication.Commands.PetTypeManagement.AddPetType;

public class AddPetTypeHandler(
    ISpeciesWriteRepository speciesWriteRepo,
    ILogger<AddPetTypeHandler> logger) : ICommandHandler<Species, AddPetTypeComand>
{
    private ILogger<AddPetTypeHandler> _logger = logger;
    private readonly ISpeciesWriteRepository _speciesWriteRepo = speciesWriteRepo;
    public async Task<Result<Species>> Handle(
        AddPetTypeComand cmd,
        CancellationToken ct)
    {
        AddPetTypeCommandValidator.Validate(cmd);
        
        var createSpecies = Species.Create(SpeciesID.NewGuid(), cmd.SpeciesName);
        if (createSpecies.IsFailure)
            throw new ValidationException(createSpecies.Error);

        var species = createSpecies.Data!;

        var breeds = CreateBreedsProcess(cmd.BreedList);
        
        species.AddBreeds(breeds);

        await _speciesWriteRepo.AddAndSaveAsync(species, ct);

        _logger.LogInformation("PetType with id:{Id} added successfully!", species.Id);

        return Result.Ok(species);
    }

    private List<Breed> CreateBreedsProcess(IEnumerable<NewBreedDto> breedDtos)
    {
        var breeds = new List<Breed>();

        foreach (var breed in breedDtos)
        {
            var createBreed = Breed.Create(BreedID.NewGuid(), breed.Name, breed.Description);
            if (createBreed.IsFailure)
                throw new ValidationException(createBreed.Error);

            breeds.Add(createBreed.Data!);
        }
        return breeds;
    }
}
