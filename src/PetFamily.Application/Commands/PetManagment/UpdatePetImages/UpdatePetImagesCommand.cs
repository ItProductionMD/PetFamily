using PetFamily.Application.Commands.FilesManagment.Commands;

namespace PetFamily.Application.Commands.PetManagment.UpdatePetImages;

public record UpdatePetImagesCommand(
    Guid VolunteerId, 
    Guid PetId,
    List<DeleteFileCommand> DeleteCommands,
    List<UploadFileCommand> UploadCommands);
