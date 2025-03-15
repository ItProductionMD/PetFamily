using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;

public class DeleteVolunteerHandler(
    ILogger<DeleteVolunteerHandler> logger,
    IVolunteerRepository volunteerRepository,
    IFileRepository fileRepository,
    FilesProcessingQueue filesProcessingQueue,
    IOptions<FileFolders> fileFoldersOptions)
{
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<DeleteVolunteerHandler> _logger = logger;
    FilesProcessingQueue _filesProcessingQueue = filesProcessingQueue;
    FileFolders _fileFolders = fileFoldersOptions.Value;
    public async Task<Result<Guid>> Handle(Guid volunteerId, CancellationToken cancelToken)
    {
        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancelToken);
        var pets = volunteer.Pets;
        List<AppFile> imagesToDelete = [];
        foreach (var pet in pets)
        {
            imagesToDelete.AddRange(pet.Images.Select(x => new AppFile(x.Name,_fileFolders.Images)));   
        }

        await _volunteerRepository.Delete(volunteer, cancelToken);

        if(imagesToDelete.Count > 0)
        {
            await _filesProcessingQueue.DeleteChannel.Writer.WriteAsync(imagesToDelete,CancellationToken.None);
        }

        _logger.LogInformation("Hard delete volunteer with id:{Id} successful!",volunteerId);

        return Result.Ok(volunteer.Id);
    }
}
