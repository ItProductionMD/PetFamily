using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Commands;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using System.Collections.Generic;
using System.Threading;

namespace PetFamily.Application.Commands.PetManagment.UpdatePetImages;
public class UpdatePetImagesHandler(
    IFileRepository fileRepository,
    IVolunteerWriteRepository volunteerRepository,
    ILogger<UpdatePetImagesHandler> logger,
    IOptions<FileFolders> fileFolders,
    FilesProcessingQueue filesProcessingQueue) 
    : ICommandHandler<UpdateFilesResponse,UpdatePetImagesCommand>
{
    private readonly IVolunteerWriteRepository _volunteerRepository = volunteerRepository;
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly ILogger<UpdatePetImagesHandler> _logger = logger;
    private readonly FileFolders _fileFolders = fileFolders.Value;
    private readonly FilesProcessingQueue _filesProcessingQueue = filesProcessingQueue;
    public async Task<Result<UpdateFilesResponse>> Handle(
        UpdatePetImagesCommand command,
        CancellationToken cancelToken)
    {
        var getVolunteer = await _volunteerRepository.GetByIdAsync(command.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return Result.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var pet = volunteer.GetPet(command.PetId);

        var imagesToDelete = command.DeleteCommands.Select(c => Image.Create(c.StoredName).Data!).ToList();
        var deletedImages = pet.DeleteImages(imagesToDelete);
        if(command.DeleteCommands.Count > deletedImages.Count)
        {
            foreach(var commandFile in command.DeleteCommands)
            {
                if(deletedImages.Contains(commandFile.StoredName) == false)
                {
                    _logger.LogWarning("Delete image:{name} from pet with id:{Id} error!File not found",
                        commandFile.StoredName, pet.Id);
                }
            }
        }


        var imagesToAdd = command.UploadCommands.Select(c => Image.Create(c.StoredName).Data!).ToList();
        var addedImages = pet.AddImages(imagesToAdd);

        List<Error> errors = [];

        var deleteTask = DeleteFilesFromFileServer(
            errors, 
            deletedImages,
            pet.Id, 
            cancelToken);

        var uploadTask = addedImages.Count > 0
            ? FileUploadResponses(command.UploadCommands, errors,addedImages, pet, cancelToken)
            : Task.FromResult(new List<FileUploadResponse>());

        await Task.WhenAll(deleteTask, uploadTask);

        var deletedFiles = await deleteTask;
        var uploadedFilesResponse = await uploadTask;

        var deletedFilesResponse = FileResponseFactory.CreateDeleteResponseList(
            deletedFiles,
            command.DeleteCommands);

        try
        {
            //throw new ApplicationException("Test exception");
            await _volunteerRepository.SaveWithRetry(volunteer, cancelToken);

            var response = new UpdateFilesResponse(deletedFilesResponse, uploadedFilesResponse);

            if (uploadedFilesResponse.Any(r => r.IsUploaded) || deletedFilesResponse.Any(r => r.IsDeleted))
                return Result.Ok(response);

            return Result.Fail(errors);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Update pet images - save volunteer failed!Ex{exception}", ex);

            var filesToRestore = deletedFilesResponse.Select(r => 
                new AppFile(r.Name, _fileFolders.Images)).ToList();

            var filesToDelete = uploadedFilesResponse.Select(r => 
                new AppFile(r.StoredName, _fileFolders.Images)).ToList();

            await _fileRepository.RestoreFileListAsync(filesToRestore,CancellationToken.None);

            await _filesProcessingQueue.DeleteChannel.Writer
                .WriteAsync(filesToDelete,CancellationToken.None);

            return Result.Fail(Error.InternalServerError("Update images for pet unexpected failure!"));
        }
        
    }

    private async Task<List<string>> DeleteFilesFromFileServer(
        List<Error> errors,
        List<string> imagesDeletedFromPet,
        Guid petId,
        CancellationToken cancelToken)
    {
        var files = imagesDeletedFromPet.Select(i =>
            new AppFile(i, _fileFolders.Images)).ToList();

        var deleteResult = await _fileRepository.SoftDeleteFileListAsync(files, cancelToken);
        if (deleteResult.IsFailure)
        {
            errors.AddRange(deleteResult.Error);

            _logger.LogWarning("Fail delete some files from file server for pet with id:{Id}!Errors:{Errors}",
                petId, deleteResult.Error.Message);
        }
        return deleteResult.Data ?? [];
    }

    private async Task<List<FileUploadResponse>> FileUploadResponses(
        List<UploadFileCommand> uploadCommands,
        List<Error> errors,
        List<string> imagesAddedToPet,
        Pet pet,
        CancellationToken cancelToken)
    {
        var files = uploadCommands.Select(c =>
            new AppFile(
                c.StoredName,
                _fileFolders.Images,
                c.Stream,
                c.Extension,
                c.MimeType,
                c.Size)).ToList();

        var uploadResult = await _fileRepository.UploadFileListAsync(files, cancelToken);
        if (uploadResult.IsFailure)
        {
            errors.AddRange(uploadResult.Error);
            _logger.LogError("Fail upload some images to file server for pet with id:{Id}!",
                pet.Id);
        }
        var imagesUploadedToFileServer = uploadResult.Data ?? [];
        if(imagesUploadedToFileServer.Count < imagesAddedToPet.Count)
        {
            var imageNamesToDelete = imagesAddedToPet.Except(imagesUploadedToFileServer).ToList();
            var imagesToDelete = imageNamesToDelete.Select(n => Image.Create(n).Data!).ToList();
            pet.DeleteImages(imagesToDelete);
        }
        return FileResponseFactory.CreateUploadResponseList(imagesUploadedToFileServer, uploadCommands);
    }
}
