using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.PetManagement.DeletePetImages;

public class DeletePetImagesHandler(
    IVolunteerWriteRepository repository,
    IFileService fileService,
    ILogger<DeletePetImagesHandler> logger)
    : ICommandHandler<DeletePetImagesCommand>
{
    private readonly IVolunteerWriteRepository _repository = repository;
    private readonly ILogger<DeletePetImagesHandler> _logger = logger;
    private readonly IFileService _fileService = fileService;

    public async Task<UnitResult> Handle(
        DeletePetImagesCommand command,
        CancellationToken ct)
    {
        var getVolunteer = await _repository.GetByIdAsync(command.VolunteerId, ct);
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
            .Select(i => new FileDto(i, Constants.BUCKET_FOR_PET_IMAGES))
            .ToList();

        await _repository.SaveAsync(volunteer, ct);

        await _fileService.DeleteFilesUsingMessageQueue(fileDtos, ct);

        return UnitResult.Ok();
    }
}
