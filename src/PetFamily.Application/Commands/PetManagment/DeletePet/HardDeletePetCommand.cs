using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetManagment.DeletePet;

public record HardDeletePetCommand(Guid VolunteerId, Guid PetId) : ICommand;