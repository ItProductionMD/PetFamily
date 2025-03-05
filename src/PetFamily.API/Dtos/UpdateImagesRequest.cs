using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Application.Volunteers.UpdatePetImages;

namespace PetFamily.API.Dtos;

public record UpdateImagesRequest(List<IFormFile> ImagesToUpload, List<string> ImagesToDelete)
{
    public List<DeleteFileCommand> ToDeleteCommands() => 
        ImagesToDelete.Select(i => new DeleteFileCommand(i)).ToList();

    public List<UploadFileCommand> ToUploadCommands() =>
        ImagesToUpload.Select(f =>
            new UploadFileCommand(
                f.FileName,
                f.ContentType.ToLowerInvariant(),
                f.Length,
                UploadFileCommand.GetFullExtension(f.FileName),
                f.OpenReadStream())).ToList();

    public UpdatePetImagesCommand ToUpdateCommand(Guid VolunteerId,Guid PetId) => 
        new(VolunteerId, PetId, ToDeleteCommands(), ToUploadCommands());
}


