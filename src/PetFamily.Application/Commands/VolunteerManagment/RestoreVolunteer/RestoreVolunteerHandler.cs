using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.Commands.SharedCommands;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.VolunteerManagment.RestoreVolunteer;

public class RestoreVolunteerHandler(
ILogger<RestoreVolunteerHandler> logger,
IVolunteerWriteRepository volunteerRepository,
IFileRepository fileRepository,
IOptions<FileFolders> fileFoldersOptions):ICommandHandler<VolunteerIdCommand>
{
    private readonly FileFolders _fileFolders = fileFoldersOptions.Value;
    private readonly IVolunteerWriteRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<RestoreVolunteerHandler> _logger = logger;
    private readonly IFileRepository _fileRepository = fileRepository;
    public async Task<UnitResult> Handle(VolunteerIdCommand command, CancellationToken cancelToken)
    {
        //TODO INDRODUCE THE UNIT OF WORK FOR TRANSACTION
        var getVolunteer = await _volunteerRepository.GetByIdAsync(command.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;
        volunteer.Restore();

        var pets = volunteer.Pets;

        List<AppFile> imagesToRestore = [];

        foreach (var pet in pets)
            imagesToRestore.AddRange(pet.Images
                .Select(x => new AppFile(x.Name, _fileFolders.Images)));

        var restoreResult = await _volunteerRepository.Save(volunteer, cancelToken);
        if(restoreResult.IsFailure)
            return restoreResult;

        if (imagesToRestore.Count > 0)
        {
            var result = await _fileRepository.RestoreFileListAsync(imagesToRestore);
            if(result.Data!=null && result.Data.Count < imagesToRestore.Count)
            {
                var unrestoredFiles = imagesToRestore
                    .Select(f => f.Name)
                    .Except(result.Data).ToList();

                string namesMessage = string.Join(";", result.Data);
                _logger.LogError("Some pet images were not restored for volunteer with Id:{id}" +
                    "||Images:{images}",
                    volunteer.Id, namesMessage);

                _logger.LogInformation("Delete unrestored pet images from pets of volunteer with " +
                    "id:{id}", volunteer.Id);

                volunteer.DeleteImagesFromPets(unrestoredFiles);

                var saveResult = await _volunteerRepository.Save(volunteer, cancelToken);
                if (saveResult.IsFailure)
                {
                    _logger.LogCritical("Save Volunteer with id:{Id} after updating unrestoring Images Fail!" +
                        "Some unrestored images were not deleted from volunteer!",
                        volunteer.Id);
                    return saveResult;//todo RolleBack
                }
            }
        }

        _logger.LogInformation("Restore volunteer with Id:{Id} successful",command.VolunteerId);

        return UnitResult.Ok();
    }
}
