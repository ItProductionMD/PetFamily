using PetFamily.Application.Abstractions.CQRS;

namespace PetSpecies.Application.Commands.DeleteBreed;

public record DeleteBreedCommand(Guid SpeciesId, Guid BreedId) : ICommand;
