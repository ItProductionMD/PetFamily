using FileStorage.Public.Dtos;
using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Volunteers.Application.Commands.PetManagement.AddPetImages;

public class AddPetImagesCommand : ICommand
{
    public Guid VolunteerId { get; set; }
    public Guid PetId { get; set; }
    public List<UploadFileDto> UploadFileDtos { get; set; } = [];

    public AddPetImagesCommand(
        Guid volunteerId,
        Guid petId,
        List<UploadFileDto> uploadFileCommands)
    {
        VolunteerId = volunteerId;
        PetId = petId;
        UploadFileDtos = uploadFileCommands;
    }

    public List<string> GetImageNames() =>
        UploadFileDtos
            .Select(c => c.StoredName)
            .ToList();
}
