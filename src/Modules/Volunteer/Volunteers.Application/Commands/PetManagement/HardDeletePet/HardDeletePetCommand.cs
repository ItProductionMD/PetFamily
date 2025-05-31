using PetFamily.Application.Abstractions.CQRS;

namespace Volunteers.Application.Commands.PetManagement.DeletePet;

public record HardDeletePetCommand(Guid VolunteerId, Guid PetId) : ICommand;