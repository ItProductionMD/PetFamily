using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetSpecies.Application.Commands.AddSpecies;
using PetSpecies.Application.Commands.CommandsDtos;
using PetSpecies.Application.IRepositories;
using PetSpecies.Domain;


namespace PetFamily.SharedApplication.Commands.PetTypeManagement.AddPetType;

public class AddPetTypeHandler(
    ISpeciesWriteRepository speciesWriteRepo,
    ILogger<AddPetTypeHandler> logger) : ICommandHandler<Species, AddPetTypeComand>
{
    public async Task<Result<Species>> Handle(AddPetTypeComand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var species = Species.Create(SpeciesID.NewGuid(), cmd.SpeciesName).Data!;

        var breeds = CreateBreedsProcess(cmd.BreedList);

        species.AddBreeds(breeds);

        await speciesWriteRepo.AddAndSaveAsync(species, ct);

        logger.LogInformation("PetType with id:{Id} added successfully!", species.Id);

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
