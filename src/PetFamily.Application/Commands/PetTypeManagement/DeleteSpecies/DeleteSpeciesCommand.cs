using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetTypeManagement.DeleteSpecies;

public record DeleteSpeciesCommand(Guid SpeciesId) : ICommand;
