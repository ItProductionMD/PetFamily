using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.PetManagment.ChangeMainPetImage;

public class ChangePetMainImageHandler(
    IVolunteerWriteRepository volunteerWriteRepository,
    ILogger<ChangePetMainImageHandler> logger) : ICommandHandler<ChangePetMainImageCommand>
{
    private readonly ILogger<ChangePetMainImageHandler> _logger = logger;
    private readonly IVolunteerWriteRepository _writeRepository = volunteerWriteRepository;

    public async Task<UnitResult> Handle(ChangePetMainImageCommand cmd, CancellationToken cancelToken)
    {
        var validatioResult = ChangeMainPetImageValidation.Validate(cmd);
        if (validatioResult.IsFailure)
        {
            _logger.LogWarning("Validate changeMainPetImageCommand failure!Error:{Error}",
                validatioResult.ValidationMessagesToString());

            return validatioResult;
        }

        var getVolunteer = await _writeRepository.GetByIdAsync(cmd.VolunteerId, cancelToken);
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

        var initialMainImage = pet.Images.FirstOrDefault();

        var changeImageResult = pet.ChangeMainPhoto(cmd.imageName);
        if (changeImageResult.IsFailure)
        {
            _logger.LogWarning("Image with name:{name} not found", cmd.imageName);
            return changeImageResult;
        }

        if (initialMainImage != null && initialMainImage.Name == pet.Images[0].Name)
        {
            _logger.LogWarning("Changed Pet main image is the same!Name:{imageName}", cmd.imageName);
            return UnitResult.Ok();
        }

        var saveResult = await _writeRepository.Save(volunteer, cancelToken);

        return saveResult;
    }
}
