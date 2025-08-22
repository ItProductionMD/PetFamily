using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.VolunteerManagement.RestoreVolunteer;

public class RestoreVolunteerHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    IFileService fileService,
    ILogger<RestoreVolunteerHandler> logger) : ICommandHandler<RestoreVolunteerCommand>
{
    public async Task<UnitResult> Handle(RestoreVolunteerCommand cmd, CancellationToken ct)
    {
        //TODO INDRODUCE THE UNIT OF WORK FOR TRANSACTION
        var getVolunteer = await volunteerWriteRepo.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;
        volunteer.Restore();

        var pets = volunteer.Pets;

        List<FileDto> imagesToRestore = [];

        foreach (var pet in pets)
            imagesToRestore.AddRange(pet.Images
                .Select(x => new FileDto(x.Name, Constants.BUCKET_FOR_PET_IMAGES)));

        var restoreResult = await volunteerWriteRepo.SaveAsync(volunteer, ct);
        if (restoreResult.IsFailure)
            return restoreResult;

        if (imagesToRestore.Count > 0)
        {
            var result = await fileService.RestoreFilesAsync(imagesToRestore);
            if (result.Data != null && result.Data.Count < imagesToRestore.Count)
            {
                var unrestoredFiles = imagesToRestore
                    .Select(f => f.Name)
                    .Except(result.Data).ToList();

                string namesMessage = string.Join(";", result.Data);
                logger.LogError("Some pet images were not restored for volunteer with Id:{id}" +
                    "||Images:{images}",
                    volunteer.Id, namesMessage);

                logger.LogInformation("Delete unrestored pet images from pets of volunteer with " +
                    "id:{id}", volunteer.Id);

                volunteer.DeleteImagesFromPets(unrestoredFiles);

                var saveResult = await volunteerWriteRepo.SaveAsync(volunteer, ct);
                if (saveResult.IsFailure)
                {
                    logger.LogCritical("Save Volunteer with id:{Id} after updating restoring Images Fail!" +
                        "Some unrestored images were not deleted from volunteer!",
                        volunteer.Id);
                    return saveResult;//todo RolleBack?
                }
            }
        }
        logger.LogInformation("Restore volunteer with Id:{Id} successful", cmd.VolunteerId);

        return UnitResult.Ok();
    }
}
