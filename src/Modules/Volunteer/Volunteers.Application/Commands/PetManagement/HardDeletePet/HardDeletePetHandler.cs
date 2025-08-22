using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;


namespace Volunteers.Application.Commands.PetManagement.DeletePet;

public class HardDeletePetHandler(
    IVolunteerWriteRepository repository,
    IFileService fileService,
    ILogger<HardDeletePetHandler> logger) : ICommandHandler<HardDeletePetCommand>
{
    public async Task<UnitResult> Handle(HardDeletePetCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var getVolunteer = await repository.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == cmd.PetId);
        if (pet == null)
        {
            logger.LogError("Pet with id:{petId}  for volunteer with id:{volunteerId} not found!",
                cmd.PetId, cmd.VolunteerId);

            return UnitResult.Fail(Error.NotFound($"Pet with id:{cmd.PetId}"));
        }

        volunteer.HardDeletePet(pet);

        var result = await repository.SaveAsync(volunteer, ct);
        if (result.IsFailure)
            return result;

        if (pet.Images.Count > 0)
        {
            var filesToDelete = pet.Images
                .Select(i => new FileDto(i.Name, Constants.BUCKET_FOR_PET_IMAGES))
                .ToList();

            await fileService.DeleteFilesUsingMessageQueue(filesToDelete, ct);
        }

        logger.LogInformation("Pet with id:{petId} from volunteer with id:{volunteerId} was deleted" +
            "(hard) successful!", cmd.PetId, cmd.VolunteerId);

        return UnitResult.Ok();
    }
}
