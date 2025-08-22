using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.PetManagement.AddPetImages;

public class AddPetImagesHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    IFileService fileService,
    IUploadFileDtoValidator fileValidator,
    IOptions<PetImagesValidatorOptions> fileValidatorOptions,
    ILogger<AddPetImagesHandler> logger) : ICommandHandler<List<FileUploadResponse>, AddPetImagesCommand>
{
    private readonly PetImagesValidatorOptions _fileValidatorOptions = fileValidatorOptions.Value;

    public async Task<Result<List<FileUploadResponse>>> Handle(
        AddPetImagesCommand cmd,
        CancellationToken ct)
    {
        var validateFiles = fileValidator.ValidateFiles(
            cmd.UploadFileDtos,
            _fileValidatorOptions);

        if (validateFiles.IsFailure)
        {
            logger.LogWarning("Error validate files for volunteer with id:{Id}! Errors:{errors}",
                cmd.VolunteerId, validateFiles.ValidationMessagesToString());
            return validateFiles;
        }

        var getVolunteer = await volunteerWriteRepo.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == cmd.PetId);
        if (pet == null)
        {
            logger.LogError("Pet with Id:{pId} for volunteer with id:{vId} not found!",
                cmd.PetId, cmd.VolunteerId);
            return UnitResult.Fail(Error.NotFound("Pet"));
        }

        pet.AddImages(cmd.GetImageNames());

        var uploadFileDtos = cmd.UploadFileDtos;

        //save files to server - if at least one file is uploaded successfully, the result will be Ok. 
        var filesUploadResult = await fileService.UploadFilesAsync(uploadFileDtos, ct);
        if (filesUploadResult.IsFailure)
            return filesUploadResult;

        var unsavedFiles = filesUploadResult.Data!
            .Where(f => f.IsUploaded == false)
            .Select(f => f.StoredName)
            .ToList();

        //delete from pet unsaved files
        if (unsavedFiles.Count > 0)
        {
            logger.LogError("Some files were not uploaded to server!Files:{fileNames}",
                string.Join(',', unsavedFiles));

            pet.DeleteImages(unsavedFiles);
        }

        var saveResult = await volunteerWriteRepo.SaveAsync(volunteer, ct);
        if (saveResult.IsFailure)
        {
            var fileDtosToDelete = uploadFileDtos
                .Select(f => new FileDto(f.StoredName, f.Folder))
                .ToList();

            await fileService.DeleteFilesUsingMessageQueue(fileDtosToDelete, ct);

            logger.LogError("Update pet images for volunteer with id:{vId} for pet with id:{pId}" +
                "error while save volunteer!",
                cmd.VolunteerId, cmd.PetId);

            return UnitResult.Fail(Error.InternalServerError("Unexpected error while saving data!"));
        }

        return filesUploadResult;
    }
}
