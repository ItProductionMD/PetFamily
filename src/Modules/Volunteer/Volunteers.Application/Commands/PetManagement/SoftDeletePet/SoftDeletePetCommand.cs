using PetFamily.Application.Abstractions.CQRS;

namespace Volunteers.Application.Commands.PetManagement.DeletePet;

public record SoftDeletePetCommand(Guid VolunteerId, Guid PetId) : ICommand;

