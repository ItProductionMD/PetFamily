using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment.Commands;

namespace PetFamily.Application.Commands.PetManagment.AddPetImages;

public class AddPetImagesCommand : ICommand
{
    public Guid VolunteerId { get; set; }
    public Guid PetId { get; set; }
    public List<UploadFileCommand> UploadFileCommands { get; set; } = [];
    public AddPetImagesCommand(Guid volunteerId, Guid petId, List<UploadFileCommand> uploadFileCommands)
    {
        VolunteerId = volunteerId;
        PetId = petId;
        UploadFileCommands = uploadFileCommands;
    }

    public List<string> GetImageNames() =>
        UploadFileCommands
            .Select(c => c.StoredName)
            .ToList();
}
