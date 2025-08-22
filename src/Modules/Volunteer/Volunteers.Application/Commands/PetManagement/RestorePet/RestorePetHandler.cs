using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;
using Volunteers.Domain;

namespace Volunteers.Application.Commands.PetManagement.RestorePet;

public class RestorePetHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    IFileService fileService,
    ILogger<RestorePetHandler> logger) : ICommandHandler<RestorePetCommand>
{
    public async Task<UnitResult> Handle(RestorePetCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var getVolunteer = await volunteerWriteRepo.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        Volunteers.Domain.Volunteer volunteer = getVolunteer.Data!;

        var restoredPet = volunteer.RestorePet(cmd.PetId);
        if (restoredPet.IsFailure)
            return restoredPet;

        var saveResult = await volunteerWriteRepo.SaveAsync(volunteer, ct);
        if (saveResult.IsFailure)
            return saveResult;

        Pet pet = volunteer.Pets.First(p => p.Id == cmd.PetId);
        //restore images
        if (pet.Images.Count > 0)
        {
            var filesToRestore = pet.Images
                .Select(i => new FileDto(i.Name, Constants.BUCKET_FOR_PET_IMAGES))
                .ToList();

            var restoreFiles = await fileService.RestoreFilesAsync(filesToRestore);
            if (restoreFiles.IsFailure)
            {
                logger.LogError("Images for pet with Id:{petId} cannot be restored!", pet.Id);
                return UnitResult.Fail(restoreFiles.Error);
            }
            if (restoreFiles.Data!.Count != pet.Images.Count)
                logger.LogError("Some images for pet with Id:{petId} cannot be restored!", pet.Id);
            else
                logger.LogInformation("Restore Pet with id:{Id} successfully!", pet.Id);
        }
        return UnitResult.Ok();
    }
}
