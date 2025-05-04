using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetManagment.ChangeMainPetImage;

public record ChangePetMainImageCommand(Guid VolunteerId,Guid PetId,string imageName):ICommand;

