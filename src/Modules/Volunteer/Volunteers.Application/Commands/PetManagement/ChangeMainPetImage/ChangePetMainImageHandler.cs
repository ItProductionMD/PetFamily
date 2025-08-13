using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.PetManagement.ChangeMainPetImage;

public class ChangePetMainImageHandler(
    IVolunteerWriteRepository repository,
    ILogger<ChangePetMainImageHandler> logger) : ICommandHandler<ChangePetMainImageCommand>
{
    public async Task<UnitResult> Handle(ChangePetMainImageCommand cmd, CancellationToken ct)
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

        var initialMainImage = pet.Images.FirstOrDefault();

        var changeImageResult = pet.ChangeMainPhoto(cmd.imageName);
        if (changeImageResult.IsFailure)
        {
            logger.LogWarning("Image with name:{name} not found", cmd.imageName);
            return changeImageResult;
        }

        if (initialMainImage != null && initialMainImage.Name == pet.Images[0].Name)
        {
            logger.LogWarning("Changed Pet main image is the same!Name:{imageName}", cmd.imageName);
            return UnitResult.Ok();
        }

        var saveResult = await repository.SaveAsync(volunteer, ct);

        return saveResult;
    }
}
