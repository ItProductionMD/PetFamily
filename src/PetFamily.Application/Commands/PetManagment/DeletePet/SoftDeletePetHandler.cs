using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;
using PetFamily.Domain.DomainError;
using PetFamily.Application.Commands.FilesManagment;
using Microsoft.Extensions.Options;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using static PetFamily.Application.Commands.PetManagment.DeletePet.DeletePetValidation;

namespace PetFamily.Application.Commands.PetManagment.DeletePet;

public class SoftDeletePetHandler(
    IVolunteerWriteRepository volunteerWriteRepository,
    IFileRepository fileRepository,
    FilesProcessingQueue filesProcessingQueue,
    IOptions<FileFolders> fileOptions,
    ILogger<SoftDeletePetHandler> logger) : ICommandHandler<SoftDeletePetCommand>
{
    private readonly IVolunteerWriteRepository _volunteerWriteRepository = volunteerWriteRepository;
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly ILogger<SoftDeletePetHandler> _logger = logger;
    private readonly FileFolders _fileFolders = fileOptions.Value;
    private readonly FilesProcessingQueue filesQueue = filesProcessingQueue;

    public async Task<UnitResult> Handle(SoftDeletePetCommand cmd, CancellationToken cancelToken)
    {
        var validation = Validate(cmd);
        if (validation.IsFailure)
        {
            _logger.LogWarning("SoftDelete pet with id:{Id} validation errors:{Errors}",
                cmd.PetId, validation.ValidationMessagesToString());

            return validation;
        }

        var getVolunteer = await _volunteerWriteRepository.GetByIdAsync(cmd.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == cmd.PetId);
        if (pet == null)
        {
            _logger.LogError("Pet with id:{petId}  for volunteer with id:{volunteerId} not found!",
                cmd.PetId, cmd.VolunteerId);

            return UnitResult.Fail(Error.NotFound($"Pet with id:{cmd.PetId}"));
        }

        volunteer.SoftDeletePet(pet);

        var result = await _volunteerWriteRepository.Save(volunteer, cancelToken);
        if (result.IsFailure)
            return result;

        if (pet.Images.Count > 0)
        {
            var filesToDelete = pet.Images
                .Select(i => new AppFileDto(i.Name, _fileFolders.Images))
                .ToList();

            await filesQueue.DeleteChannel.Writer.WriteAsync(filesToDelete);
        }

        _logger.LogInformation("Pet with id:{petId} from volunteer with id:{volunteerId} was deleted" +
            "(soft) successful!", cmd.PetId, cmd.VolunteerId);

        return UnitResult.Ok();
    }
}
