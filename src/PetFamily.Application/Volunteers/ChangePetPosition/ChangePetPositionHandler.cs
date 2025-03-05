
using Microsoft.Extensions.Logging;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Volunteers.ChangePetPosition;
public class ChangePetPositionHandler(
    IVolunteerRepository volunteerRepository,
    ILogger<ChangePetPositionHandler> logger)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<ChangePetPositionHandler> _logger = logger;
    public async Task<Result<List<ChangePetPositionResponse>>> Handle(
        ChangePetPositionCommand command,
        CancellationToken cancellationToken)
    {
        var volunteer = await _volunteerRepository.GetByIdAsync(command.VolunteerId, cancellationToken);
   
        var getSerialNumber = PetSerialNumber.Create(command.NewPetSerialNumber,volunteer);
        if (getSerialNumber.IsFailure)
        {
            _logger.LogError("Volunteer with id:{id} can't create a serial number for the pet from" +
                " number:{number}! errors:{Errors}",
                command.VolunteerId,command.NewPetSerialNumber, getSerialNumber.ToErrorMessages());
            return Result.Fail(getSerialNumber.Errors!);
        }
        var serialNumber = getSerialNumber.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == command.PetId);
        if(pet == null)
        {
            _logger.LogError("Pet with id:{petId} not found in volunteer with id:{volunteerId}",
                command.PetId, command.VolunteerId);
            return Result.Fail(Error.NotFound($"Pet with id:{command.PetId}"));
        }
        volunteer.MovePetSerialNumber(pet,serialNumber);

        await _volunteerRepository.Save(volunteer, cancellationToken);

        var response = volunteer.Pets
            .Select(p => new ChangePetPositionResponse(p.Id,p.SerialNumber.Value))
            .OrderBy(r => r.SerialNumber)
            .ToList();

        return Result.Ok(response);
    }
}


