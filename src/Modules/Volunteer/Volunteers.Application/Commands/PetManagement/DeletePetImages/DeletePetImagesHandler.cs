using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.PetManagement.DeletePetImages;

public class DeletePetImagesHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    IFileService fileService,
    ILogger<DeletePetImagesHandler> logger) : ICommandHandler<DeletePetImagesCommand>
{
    public async Task<UnitResult> Handle(
        DeletePetImagesCommand command,
        CancellationToken ct)
    {
        var getVolunteer = await volunteerWriteRepo.GetByIdAsync(command.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == command.PetId);
        if (pet == null)
        {
            logger.LogError("Pet with Id:{pId} for volunteer with id:{vId} not found!",
                command.PetId, command.VolunteerId);
            return UnitResult.Fail(Error.NotFound("Pet"));
        }
        var deletedFiles = pet.DeleteImages(command.imageNames);
        if (deletedFiles.Count == 0)
        {
            logger.LogError("Images to delete for Pet with Id:{pId} for volunteer with id:{vId} not found!",
                 command.PetId, command.VolunteerId);
            return UnitResult.Fail(Error.NotFound("Images"));
        }
        var fileDtos = command.imageNames
            .Select(i => new FileDto(i, Constants.BUCKET_FOR_PET_IMAGES))
            .ToList();

        await volunteerWriteRepo.SaveAsync(volunteer, ct);

        await fileService.DeleteFilesUsingMessageQueue(fileDtos, ct);

        return UnitResult.Ok();
    }
}
