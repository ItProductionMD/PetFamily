using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetTypeManagment.DeleteSpecies;

public record DeleteSpeciesCommand(Guid SpeciesId) : ICommand;
