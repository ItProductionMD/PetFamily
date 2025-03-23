using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.Commands.SharedCommands;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;

public class DeleteVolunteerHandler(
    ILogger<DeleteVolunteerHandler> logger,
    IVolunteerWriteRepository volunteerRepository,
    IFileRepository fileRepository,
    FilesProcessingQueue filesProcessingQueue,
    IOptions<FileFolders> fileFoldersOptions) : ICommandHandler<Guid,VolunteerIdCommand>
{
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly IVolunteerWriteRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<DeleteVolunteerHandler> _logger = logger;
    FilesProcessingQueue _filesProcessingQueue = filesProcessingQueue;
    FileFolders _fileFolders = fileFoldersOptions.Value;
    public async Task<Result<Guid>> Handle(VolunteerIdCommand command, CancellationToken cancelToken)
    {
        var getVolunteer = await _volunteerRepository.GetByIdAsync(command.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return Result.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        List<AppFile> imagesToDelete = [];

        foreach (var pet in volunteer.Pets)
            imagesToDelete.AddRange(pet.Images.Select(x => new AppFile(x.Name,_fileFolders.Images)));   
        

        await _volunteerRepository.Delete(volunteer, cancelToken);

        if(imagesToDelete.Count > 0)
            await _filesProcessingQueue.DeleteChannel.Writer.WriteAsync(imagesToDelete);       

        _logger.LogInformation("Hard delete volunteer with id:{Id} successful!", command.VolunteerId);

        return Result.Ok(volunteer.Id);
    }
}
