using PetFamily.Application.FilesManagment.Commands;

namespace PetFamily.Application.Volunteers.UpdatePetImages;

public record UpdatePetImagesCommand(
    Guid VolunteerId, 
    Guid PetId,
    List<DeleteFileCommand> DeleteCommands,
    List<UploadFileCommand> UploadCommands);
