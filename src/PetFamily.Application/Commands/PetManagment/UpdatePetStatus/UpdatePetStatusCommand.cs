using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetManagment.UpdatePetStatus;

public record UpdatePetStatusCommand(Guid VolunteerId, Guid PetId, int HelpStatus) : ICommand;

