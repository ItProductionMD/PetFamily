using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.FilesManagment.Dtos;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Volunteers.RestoreVolunteer;

public class RestoreVolunteerHandler(
ILogger<RestoreVolunteerHandler> logger,
IVolunteerRepository volunteerRepository,
IFileRepository fileRepository,
IOptions<FileFolders> fileFoldersOptions)
{
    private readonly FileFolders _fileFolders = fileFoldersOptions.Value;
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<RestoreVolunteerHandler> _logger = logger;
    private readonly IFileRepository _fileRepository = fileRepository;
    public async Task<UnitResult> Handle(Guid volunteerId, CancellationToken cancelToken)
    {
        //TODO INDRODUCE UNIT OF WORK FO TRANSACTION
        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancelToken);
        
        volunteer.Restore();

        var pets = volunteer.Pets;

        List<AppFile> imagesToRestore = [];

        foreach (var pet in pets)
            imagesToRestore.AddRange(pet.Images
                .Select(x => new AppFile(x.Name, _fileFolders.Images)));

        await _volunteerRepository.Save(volunteer, cancelToken);

        if (imagesToRestore.Count > 0)
        {
            var result = await _fileRepository.RestoreFileListAsync(imagesToRestore, CancellationToken.None);
            if(result.Data!=null && result.Data.Count<imagesToRestore.Count)
            {
                var unrestoredFiles = imagesToRestore
                    .Select(f => f.Name)
                    .Except(result.Data).ToList();

                string namesMessage = string.Join(";", result.Data);
                _logger.LogError("Some pet images were not restored for volunteer with Id:{id}||Images:{images}",
                    volunteer.Id, namesMessage);

                _logger.LogInformation("Delete unrestored pet images from pets of volunteer with id:{id}", volunteer.Id);

                volunteer.DeleteImagesFromPets(unrestoredFiles);

                await _volunteerRepository.Save(volunteer, cancelToken);
            }
        }

        _logger.LogInformation("Restore volunteer with Id:{Id} successful",volunteerId);

        return UnitResult.Ok();
    }
}
