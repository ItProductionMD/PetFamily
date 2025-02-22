using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Application.FilesManagment.Dtos;
using PetFamily.Application.Volunteers;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using System.Collections.Generic;
using System.Threading;

namespace PetFamily.Application.Volunteers.UpdatePetImages;
public class UpdatePetImagesHandler(
    IFileRepository fileRepository,
    IVolunteerRepository volunteerRepository,
    ILogger<UpdatePetImagesHandler> logger)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly ILogger<UpdatePetImagesHandler> _logger = logger;
    public async Task<Result<UpdateFilesResponse>> Handle(
        string folder,
        Guid volunteerId,
        Guid petId,
        List<UploadFileCommand> uploadCommands,
        List<DeleteFileCommand> deleteCommands,
        CancellationToken cancellationToken)
    {
        var getVolunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Volunteer with id:{volunteerId}", volunteerId);
            return Result.Fail(Error.NotFound($"Volunteer with id:{volunteerId} not found!"));
        }
        var volunteer = getVolunteer.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == petId);
        if (pet == null)
        {
            _logger.LogError("Pet with id:{petId} not found!", petId);
            return Result.Fail(Error.NotFound($"Pet with id:{petId} not found!"));
        }
        List<Error> errors = [];
        List<FileDeleteResponse> deleteResponse = [];
        if (deleteCommands.Count > 0)
        {
            var deleteResult = await _fileRepository.DeleteFileListAsync(
                     folder,
                     deleteCommands,
                     cancellationToken);
            if (deleteResult.IsFailure)
            {
                errors.AddRange(deleteResult.Errors);
                _logger.LogError("Fail delete some files from pet with id:{Id}!Errors:{Errors}",
                    pet.Id, deleteResult.ConcateErrorMessages());
            }
            var imagesToDelete = deleteResult.Data ?? [];
            pet.DeleteImages(imagesToDelete);

            deleteResponse = FileResponseFactory.CreateDeleteResponseList(imagesToDelete, deleteCommands);
        }
        List<FileUploadResponse> uploadResponse = [];
        if (uploadCommands.Count > 0)
        {
            var uploadResult = await _fileRepository.UploadFileListAsync(
                folder,
                uploadCommands,
                cancellationToken);
            if (uploadResult.IsFailure)
            {
                errors.AddRange(uploadResult.Errors);
                _logger.LogError("Fail upload some images to pet with id:{Id}!Errors:{Errors}",
                    petId, uploadResult.ConcateErrorMessages());
            }
            var imagesToAdd = uploadResult.Data ?? [];
            pet.AddImages(imagesToAdd);

            uploadResponse = FileResponseFactory.CreateUploadResponseList(imagesToAdd, uploadCommands);
        }
        try
        {
            await _volunteerRepository.Save(volunteer, cancellationToken);
        }
        catch (Exception ex)
        {
            //TODO Delete from server uploaded photo /background service?
            return Result.Fail(Error.Exception(ex));
        }
        var response = new UpdateFilesResponse(deleteResponse, uploadResponse);

        if (uploadResponse.Any(r => r.IsUploaded) || deleteResponse.Any(r => r.IsDeleted))
            return Result.Ok(response);

        return Result.Fail(errors);
    }
}
