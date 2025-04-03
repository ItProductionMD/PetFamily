using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetManagment.ChangeMainPetImage;

public record ChangeMainPetImageCommand(Guid VolunteerId,Guid PetId,string imageName):ICommand;

