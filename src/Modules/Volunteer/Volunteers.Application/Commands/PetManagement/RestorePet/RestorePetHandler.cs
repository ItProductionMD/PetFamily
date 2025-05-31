using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;
using Volunteers.Domain;

namespace Volunteers.Application.Commands.PetManagement.RestorePet;

public class RestorePetHandler(
    IVolunteerWriteRepository repository,
    IFileService fileService,
    ILogger<RestorePetHandler> logger) : ICommandHandler<RestorePetCommand>
{
    private readonly IVolunteerWriteRepository _repository = repository;
    private readonly ILogger<RestorePetHandler> _logger = logger;
    private readonly IFileService _fileService = fileService;

    public async Task<UnitResult> Handle(RestorePetCommand cmd, CancellationToken ct)
    {
        var validate = RestorePetCommandValidation.Validate(cmd);
        if (validate.IsFailure)
        {
            _logger.LogWarning("Restore pet with id:{Id},Validate petCommand Errors:{Errors}",
                cmd.PetId, validate.ValidationMessagesToString());

            return validate;
        }

        var getVolunteer = await _repository.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        Volunteers.Domain.Volunteer volunteer = getVolunteer.Data!;

        var restoredPet = volunteer.RestorePet(cmd.PetId);
        if (restoredPet.IsFailure)
            return restoredPet;

        var saveResult = await _repository.Save(volunteer, ct);
        if (saveResult.IsFailure)
            return saveResult;

        Pet pet = volunteer.Pets.First(p => p.Id == cmd.PetId);
        //restore images
        if (pet.Images.Count > 0)
        {
            var filesToRestore = pet.Images
                .Select(i => new FileDto(i.Name, Constants.BUCKET_FOR_PET_IMAGES))
                .ToList();

            var restoreFiles = await _fileService.RestoreFilesAsync(filesToRestore);
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
