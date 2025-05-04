using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.IRepositories;
using PetFamily.Application.SharedValidations;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.PetManagment.AddPetImages;

public class AddPetImagesHandler(
    IVolunteerWriteRepository volunteerWriteRepository,
    IFileRepository fileRepository,
    ILogger<AddPetImagesHandler> logger,
    IOptions<FileFolders> filePathOptions,
    IOptions<FileValidatorOptions> fileValidatorOptions,
    FilesProcessingQueue filesProcessingQueue) 
    : ICommandHandler<List<FileUploadResponse>, AddPetImagesCommand>
{
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly FileFolders _fileFolders = filePathOptions.Value;
    private readonly IVolunteerWriteRepository _volunteerWriteRepository = volunteerWriteRepository;
    private readonly ILogger<AddPetImagesHandler> _logger = logger;
    private readonly FileValidatorOptions _fileValidatorOptions = fileValidatorOptions.Value;
    private readonly FilesProcessingQueue _filesProcessingQueue = filesProcessingQueue;

    public async Task<Result<List<FileUploadResponse>>> Handle(AddPetImagesCommand command, CancellationToken cancelToken)
    {
        var validateFiles = FileValidator.ValidateList(command.UploadFileCommands, _fileValidatorOptions);
        if (validateFiles.IsFailure)
        {
            _logger.LogWarning("Error validate files for volunteer with id:{Id}! Errors:{errors}",
                command.VolunteerId, validateFiles.ValidationMessagesToString());
            return validateFiles;
        }

        var getVolunteer = await _volunteerWriteRepository.GetByIdAsync(command.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == command.PetId);
        if (pet == null)
        {
            _logger.LogError("Pet with Id:{pId} for volunteer with id:{vId} not found!",
                command.PetId, command.VolunteerId);
            return UnitResult.Fail(Error.NotFound("Pet"));
        }

        pet.AddImages(command.GetImageNames());

        var fileDtos = command.UploadFileCommands
            .Select(c => c.ToAppFileDto(_fileFolders.Images))
            .ToList();

        //save files to server - if at least one file is uploaded successfully, the result will be Ok. 
        var uploadFileToServerResult = await _fileRepository.UploadFileListAsync(fileDtos, cancelToken);
        if (uploadFileToServerResult.IsFailure)
            return uploadFileToServerResult;

        var unsavedFiles = uploadFileToServerResult.Data!
            .Where(f => f.IsUploaded == false)
            .Select(f => f.StoredName)
            .ToList();

        //delete from pet unsaved files
        if (unsavedFiles.Count > 0)
        {
            _logger.LogError("Some files were not uploaded to server!Files:{fileNames}",
                string.Join(',', unsavedFiles));

            pet.DeleteImages(unsavedFiles);
        }

        var saveResult = await _volunteerWriteRepository.Save(volunteer, cancelToken);
        if (saveResult.IsFailure)
        {
            //set files to delete in queue
            await _filesProcessingQueue.DeleteChannel.Writer
            .WriteAsync(fileDtos, cancelToken);

            _logger.LogError("Update pet images for volunteer with id:{vId} for pet with id:{pId}" +
                "error while save volunteer!",
                command.VolunteerId, command.PetId);

            return UnitResult.Fail(Error.InternalServerError("Unexpected error while saving data!"));
        }

        return uploadFileToServerResult;
    }
}
