using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetTypeManagment.DeleteBreed;

public record DeleteBreedCommand(Guid SpeciesId, Guid BreedId):ICommand;
