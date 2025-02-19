using Microsoft.Extensions.Logging;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Application.FilesManagment.Dtos;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetAggregates.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using System.Collections.Generic;
using System.Threading;

namespace PetFamily.Application.Pets.UpdatePetImages;
public class UpdatePetImagesHandler(
    IPetRepository petRepository,
    IFileRepository fileRepository,
    ILogger<UpdatePetImagesHandler> logger)
{
    private readonly IPetRepository _petRepository = petRepository;
    private readonly IFileRepository _fileService = fileRepository;
    private readonly ILogger<UpdatePetImagesHandler> _logger = logger;
    public async Task<Result<UpdateFilesResponse>> Handle(
        string folder,
        Guid petId,
        List<UploadFileCommand> uploadCommands,
        List<DeleteFileCommand> deleteCommands,
        CancellationToken cancellationToken)
    {
        var getPet = await _petRepository.GetAsync(petId, cancellationToken);
        if (getPet.IsFailure)
            return Result.Fail(getPet.Errors);

        var pet = getPet.Data!;
        var deleteResult = Result.Ok(new List<FileDeleteResponse>());
        if (deleteCommands.Count > 0)
        {
            deleteResult = await DeleteFilesProcessAsync(deleteCommands, pet, folder, cancellationToken);
        }
        //------------Check if after adding images theese count will be in permited range---------//
        if (pet.IsAddImagesCountPermited(uploadCommands.Count) == false)
        {
            _logger.LogError("Uploading images count failed! Count:{Count}", uploadCommands.Count);

            return Result.Fail(Error.Custom(
                "custom.error",
                "Error add images to pet,count of images is bigger than permited",
                ErrorType.Validation,
                "Ulpoad file count"));
        }
        //------------------------------------------Upload images---------------------------------//
        var uploadResult = Result.Ok(new List<FileUploadResponse>());
        if (uploadCommands.Count > 0)
        {
            uploadResult = await UploadFilesProcessAsync(uploadCommands, pet, folder, cancellationToken);

            if (deleteResult.Data?.Count > 0 || uploadResult.Data?.Count > 0)
            {
                var savePet = await _petRepository.UpdateImages(pet, cancellationToken);
                if (savePet.IsFailure)
                {
                    _logger.LogCritical("Update pet Images ,save pet fail, its need to delete uploaded Photoes");
                    //TODO Possible to do some background server that will destroy undeleted files
                    return Result.Fail(savePet.Errors);
                }
            }
        }
        var response = new UpdateFilesResponse(deleteResult.Data ?? [], uploadResult.Data ?? []);

        var errors = new List<Error>();

        if (deleteResult.IsFailure)
        {
            string errorMessages = string.Join("; ", deleteResult.Errors.Select(r => r.Message));

            _logger.LogError("Delete images from pet failure!{errorMessages}", errorMessages);

            errors.AddRange(deleteResult.Errors);
        }
        if (uploadResult.IsFailure)
        {
            string errorMessages = string.Join("; ", uploadResult.Errors.Select(r => r.Message));

            _logger.LogError("Upload images from pet failure!{errorMessages}", errorMessages);

            errors.AddRange(uploadResult.Errors);
        }

        return errors.Count > 0
            ? Result.Fail(errors).WithData(response)
            : Result.Ok(response);

    }
    private async Task<Result<List<FileDeleteResponse>>> DeleteFilesProcessAsync(
        List<DeleteFileCommand> deleteCommands,
        Pet pet,
        string folderName,
        CancellationToken cancellationToken)
    {
        if (deleteCommands.Count == 0)
            return UnitResult.Ok();

        var imagesToDelete = ValueObjectList<Image>
                .Create(deleteCommands, (i) => Image.Create(i.StoredName)).Data!.ToList();
        //------------------------------Delete images from pet--------------------------------//
        pet.DeleteImages(imagesToDelete);
        //------------------------------Delete images from server-----------------------------//
        var deleteResult = await _fileService.DeleteFileListAsync(
        folderName,
            deleteCommands,
            cancellationToken);

        var deletedImages = deleteResult.Data ?? [];
        //-----------------------------Create deleteResponseList------------------------------//
        var deletedResponseList = FileResponseFactory.CreateDeleteResponseList(deletedImages, deleteCommands);

        return deleteResult.IsFailure
            ? Result.Fail(deleteResult.Errors).WithData(deletedResponseList)
            : Result.Ok(deletedResponseList);
    }

    private async Task<Result<List<FileUploadResponse>>> UploadFilesProcessAsync(
        List<UploadFileCommand> uploadCommands,
        Pet pet,
        string folder,
        CancellationToken cancellationToken)
    {
        //------------------------------------Upload Files to server------------------------------//
        var uploadResult = await _fileService.UploadFileListAsync(
            folder,
            uploadCommands,
            cancellationToken);

        var uploadedImages = uploadResult.Data ?? [];
        //------------------------------------Add uploaded files to Pet---------------------------//
        if (uploadedImages.Count > 0)
        {
            var isImagesAdded = pet.AddImages(uploadedImages);
        }
        //-------------------------------------Create delete responseList-------------------------//
        var responseList = FileResponseFactory.CreateUploadResponseList(uploadedImages, uploadCommands);

        return uploadResult.IsFailure
            ? Result.Fail(uploadResult.Errors).WithData(responseList)
            : Result.Ok(responseList);
    }
}
