

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.PetManagment.DeletePetImages;

public class DeletePetImagesHandler(
    IVolunteerWriteRepository volunteerWriteRepository,
    IFileRepository fileRepository,
    IOptions<FileFolders> fileFolderOptions,
    FilesProcessingQueue filesProcessingQueue,
    ILogger<DeletePetImagesHandler> logger)
    : ICommandHandler<DeletePetImagesCommand>
{
    private readonly IVolunteerWriteRepository _volunteerWriteRepository = volunteerWriteRepository;
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly ILogger<DeletePetImagesHandler> _logger = logger;
    private readonly FileFolders _fileFolder = fileFolderOptions.Value;
    private readonly FilesProcessingQueue _filesProcessingQueue = filesProcessingQueue;

    public async Task<UnitResult> Handle(
        DeletePetImagesCommand command,
        CancellationToken cancelToken)
    {
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
        var deletedFiles = pet.DeleteImages(command.imageNames);
        if (deletedFiles.Count == 0)
        {
            _logger.LogError("Images to delete for Pet with Id:{pId} for volunteer with id:{vId} not found!",
                 command.PetId, command.VolunteerId);
            return UnitResult.Fail(Error.NotFound("Images"));
        }
        var fileDtos = command.imageNames
            .Select(i => new AppFileDto(i, _fileFolder.Images))
            .ToList();

        await _volunteerWriteRepository.Save(volunteer, cancelToken);

        await _filesProcessingQueue.DeleteChannel.Writer
                .WriteAsync(fileDtos, cancelToken);

        return UnitResult.Ok();
    }
}
