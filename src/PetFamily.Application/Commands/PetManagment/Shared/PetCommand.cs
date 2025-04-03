using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetManagment.Shared;

public record PetCommand(Guid VolunteerId, Guid PetId) : ICommand;
