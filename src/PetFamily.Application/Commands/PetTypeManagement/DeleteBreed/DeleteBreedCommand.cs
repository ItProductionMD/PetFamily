using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetTypeManagement.DeleteBreed;

public record DeleteBreedCommand(Guid SpeciesId, Guid BreedId):ICommand;
