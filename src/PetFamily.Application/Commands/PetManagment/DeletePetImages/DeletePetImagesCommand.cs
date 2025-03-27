
using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment.Commands;

namespace PetFamily.Application.Commands.PetManagment.DeletePetImages;

public class DeletePetImagesCommand:ICommand
{
    public Guid VolunteerId {  get; set; }
    public Guid PetId {  get; set; }
    public List<string> imageNames = [];

    public DeletePetImagesCommand(Guid volunteerId, Guid petId, List<string> fileNames)
    {
        VolunteerId = volunteerId;
        PetId = petId;
        imageNames = fileNames;
    }
}

