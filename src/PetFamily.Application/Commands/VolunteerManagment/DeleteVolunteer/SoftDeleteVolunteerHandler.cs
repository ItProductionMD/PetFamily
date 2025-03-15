using Microsoft.Extensions.Logging;
using PetFamily.Domain.Results;
using Microsoft.Extensions.Options;
using PetFamily.Application.IRepositories;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Dtos;

namespace PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;

public class SoftDeleteVolunteerHandler(
    ILogger<SoftDeleteVolunteerHandler> logger,
    IVolunteerRepository volunteerRepository,
    FilesProcessingQueue filesProcessingQueue,
    IOptions<FileFolders> fileFoldersOptions)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<SoftDeleteVolunteerHandler> _logger = logger;
    FilesProcessingQueue _filesProcessingQueue = filesProcessingQueue;
    FileFolders _fileFolders = fileFoldersOptions.Value;
    public async Task<Result<Guid>> Handle(Guid volunteerId, CancellationToken cancellationToken)
    {
        //--------------------------------------Get Volunteer-------------------------------------//
        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);

        var pets = volunteer.Pets;

        List<AppFile> imagesToDelete = [];

        foreach (var pet in pets)
            imagesToDelete.AddRange(pet.Images
                .Select(x => new AppFile(x.Name, _fileFolders.Images)));
        
        volunteer.Delete();

        await _volunteerRepository.Save(volunteer, cancellationToken);

        if (imagesToDelete.Count > 0)
            await _filesProcessingQueue.DeleteChannel.Writer
                .WriteAsync(imagesToDelete, CancellationToken.None);
        
       _logger.LogInformation("Softdelete volunteer with id:{volunteerId} successful!", volunteerId);

        return Result.Ok(volunteer.Id);
    }
}
