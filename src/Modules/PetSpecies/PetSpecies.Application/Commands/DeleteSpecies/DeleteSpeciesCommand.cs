using PetFamily.Application.Abstractions.CQRS;

namespace PetSpecies.Application.Commands.DeleteSpecies;
public record DeleteSpeciesCommand(Guid SpeciesId) : ICommand;
