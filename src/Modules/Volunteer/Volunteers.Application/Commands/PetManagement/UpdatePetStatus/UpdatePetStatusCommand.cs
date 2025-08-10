using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Volunteers.Application.Commands.PetManagement.UpdatePetStatus;

public record UpdatePetStatusCommand(Guid VolunteerId, Guid PetId, int HelpStatus) : ICommand;

