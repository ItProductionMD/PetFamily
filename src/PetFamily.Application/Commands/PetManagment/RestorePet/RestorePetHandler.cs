using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.Commands.PetManagment.Shared;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;
using static PetFamily.Application.Commands.PetManagment.Shared.PetCommandValidation;

namespace PetFamily.Application.Commands.PetManagment.RestorePet;

public class RestorePetHandler(
    IVolunteerWriteRepository volunteerWriteRepository,
    IFileRepository fileRepository,
    IOptions<FileFolders> fileOptions,
    ILogger<RestorePetHandler> logger) : ICommandHandler<PetCommand>
{
    private readonly IVolunteerWriteRepository _volunteerWriteRepository = volunteerWriteRepository;
    private readonly ILogger<RestorePetHandler> _logger = logger;
    private readonly FileFolders fileFolders = fileOptions.Value;
    private readonly IFileRepository _fileRepository = fileRepository;

    public async Task<UnitResult> Handle(PetCommand cmd, CancellationToken cancelToken)
    {
        var validate = Validate(cmd);
        if (validate.IsFailure)
        {
            _logger.LogWarning("Restore pet with id:{Id},Validate petCommand Errors:{Errors}",
                cmd.PetId, validate.ValidationMessagesToString());

            return validate;
        }

        var getVolunteer = await _volunteerWriteRepository.GetByIdAsync(cmd.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var restorePet = volunteer.RestorePet(cmd.PetId);
        if (restorePet.IsFailure)
            return restorePet;

        var saveResult = await _volunteerWriteRepository.Save(volunteer, cancelToken);
        if (saveResult.IsFailure)
            return saveResult;

        var pet = volunteer.Pets.First(p => p.Id == cmd.PetId);
        //restore images
        if (pet.Images.Count > 0)
        {
            var filesToRestore = pet.Images
                .Select(i => new AppFileDto(i.Name, fileFolders.Images))
                .ToList();

            var restoreFiles = await _fileRepository.RestoreFileListAsync(filesToRestore);
            if (restoreFiles.IsFailure)
            {
                _logger.LogError("Images for pet with Id:{petId} cannot be restored!", pet.Id);
                return UnitResult.Fail(restoreFiles.Error);
            }
            if (restoreFiles.Data!.Count != pet.Images.Count)
                _logger.LogError("Some images for pet with Id:{petId} cannot be restored!", pet.Id);
            else
                _logger.LogInformation("Restore Pet with id:{Id} successfully!", pet.Id);
        }
        return UnitResult.Ok();
    }
}
