using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetManagment.DeletePet;

public record DeletePetCommand(Guid VolunteerId,Guid PetId) : ICommand;

