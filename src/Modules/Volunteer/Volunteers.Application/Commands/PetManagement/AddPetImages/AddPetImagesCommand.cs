using FileStorage.Public.Dtos;
using PetFamily.Application.Abstractions.CQRS;

namespace Volunteers.Application.Commands.PetManagement.AddPetImages;

public class AddPetImagesCommand : ICommand
{
    public Guid VolunteerId { get; set; }
    public Guid PetId { get; set; }
    public List<UploadFileDto> UploadFileCommands { get; set; } = [];

    public AddPetImagesCommand(
        Guid volunteerId,
        Guid petId,
        List<UploadFileDto> uploadFileCommands)
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
