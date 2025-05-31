using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.PetManagement.AddPetImages;

public class AddPetImagesHandler(
    IVolunteerWriteRepository repository,
    IFileService fileService,
    IUploadFileDtoValidator fileValidator,
    ILogger<AddPetImagesHandler> logger,
    IOptions<PetImagesValidatorOptions> fileValidatorOptions)
    : ICommandHandler<List<FileUploadResponse>, AddPetImagesCommand>
{
    private readonly IFileService _fileService = fileService;
    private readonly IVolunteerWriteRepository _repository = repository;
    private readonly ILogger<AddPetImagesHandler> _logger = logger;
    private readonly PetImagesValidatorOptions _fileValidatorOptions = fileValidatorOptions.Value;
    private readonly IUploadFileDtoValidator _fileValidator = fileValidator;

    public async Task<Result<List<FileUploadResponse>>> Handle(
        AddPetImagesCommand command,
        CancellationToken ct)
    {
        var validateFiles = _fileValidator.ValidateFiles(
            command.UploadFileCommands,
            _fileValidatorOptions);

        if (validateFiles.IsFailure)
        {
            _logger.LogWarning("Error validate files for volunteer with id:{Id}! Errors:{errors}",
                command.VolunteerId, validateFiles.ValidationMessagesToString());
            return validateFiles;
        }

        var getVolunteer = await _repository.GetByIdAsync(command.VolunteerId, ct);
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
            .Select(c => c.ToAppFileDto(Constants.BUCKET_FOR_PET_IMAGES))
            .ToList();

        //save files to server - if at least one file is uploaded successfully, the result will be Ok. 
        var filesUploadResult = await _fileService.UploadFilesAsync(fileDtos, ct);
        if (filesUploadResult.IsFailure)
            return filesUploadResult;

        var unsavedFiles = filesUploadResult.Data!
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

        var saveResult = await _repository.Save(volunteer, ct);
        if (saveResult.IsFailure)
        {
            await _fileService.DeleteFilesUsingMessageQueue(fileDtos, ct);

            _logger.LogError("Update pet images for volunteer with id:{vId} for pet with id:{pId}" +
                "error while save volunteer!",
                command.VolunteerId, command.PetId);

            return UnitResult.Fail(Error.InternalServerError("Unexpected error while saving data!"));
        }

        return filesUploadResult;
    }
}
