using PetFamily.API.Dtos;
using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Application.Pets.CreatePet;

namespace PetFamily.API.Mappers;

public static class Mapper
{
    public static AddPetCommand MapToAddPetCommand(
        this PetDto petDto,
        Guid volunteerId)
    {
        return new AddPetCommand(
           Guid.NewGuid(),
           petDto.PetName,
           petDto.DateOfBirth,
           petDto.Description,
           petDto.IsVaccinated,
           petDto.IsSterilized,
           petDto.Weight,
           petDto.Height,
           petDto.Color,
           petDto.SpeciesId,
           petDto.BreedId,
           petDto.OwnerPhoneRegion,
           petDto.OwnerPhoneNumber,
           petDto.HealthInfo,
           petDto.HelpStatus,
           petDto.City,
           petDto.Region,
           petDto.Street,
           petDto.HomeNumber,
           petDto.DonateDetails);
    }

    public static List<UploadFileCommand> MapToUploadFileCommandList(this List<IFormFile> files)
    {
        List<UploadFileCommand> uploadCommands = [];
        foreach (var file in files)
        {
            var fileExtension = UploadFileCommand.GetFullExtension(file.FileName);

            UploadFileCommand fileDto = new(
                file.FileName,
                file.ContentType.ToLowerInvariant(),
                file.Length,
                fileExtension,
                file.OpenReadStream());

            uploadCommands.Add(fileDto);
        }
        return uploadCommands;
    }

}
