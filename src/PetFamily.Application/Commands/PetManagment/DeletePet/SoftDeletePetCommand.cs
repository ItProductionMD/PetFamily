using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetManagment.DeletePet;

public record SoftDeletePetCommand(Guid VolunteerId,Guid PetId) : ICommand;

