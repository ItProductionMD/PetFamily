using Microsoft.Extensions.Logging;
using PetFamily.Domain.Results;
using Microsoft.Extensions.Options;
using PetFamily.Application.IRepositories;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.Commands.SharedCommands;

namespace PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;

public class SoftDeleteVolunteerHandler(
    ILogger<SoftDeleteVolunteerHandler> logger,
    IVolunteerWriteRepository volunteerRepository,
    FilesProcessingQueue filesProcessingQueue,
    IOptions<FileFolders> fileFoldersOptions)
{
    private readonly IVolunteerWriteRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<SoftDeleteVolunteerHandler> _logger = logger;
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
            imagesToDelete.AddRange(pet.Images.Select(x => new AppFile(x.Name, _fileFolders.Images)));
        
        volunteer.Delete();// set is delete true fore volunteer and pets

        await _volunteerRepository.Save(volunteer, cancelToken);

        if (imagesToDelete.Count > 0)
            await _filesProcessingQueue.DeleteChannel.Writer.WriteAsync(imagesToDelete);
        
       _logger.LogInformation("Softdelete volunteer with id:{volunteerId} successful!",command.VolunteerId);

        return Result.Ok(volunteer.Id);
    }
}
