using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;
using Volunteers.Domain.Enums;

namespace Volunteers.Application.Commands.PetManagement.UpdatePetStatus;

public class UpdatePetStatusHandler(
    ILogger<UpdatePetStatusHandler> logger,
    IVolunteerWriteRepository repository) : ICommandHandler<UpdatePetStatusCommand>
{
    private readonly IVolunteerWriteRepository _repository = repository;
    private readonly ILogger<UpdatePetStatusHandler> _logger = logger;

    public async Task<UnitResult> Handle(UpdatePetStatusCommand cmd, CancellationToken ct)
    {
        var validationResult = UpdatePetStatusCommandValidator.Validate(cmd);
        if (validationResult.IsFailure)
        {
            _logger.LogWarning("Validate updatePetStatusCommand for volunteer with id:{volunteerId} " +
                "and pet with id:{petId}error:{Errors}",
                cmd.VolunteerId, cmd.PetId, validationResult.ValidationMessagesToString());

            return validationResult;
        }

        var getVolunteer = await _repository.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == cmd.PetId);
        if (pet == null)
        {
            _logger.LogWarning("Pet with id:{Id} not found!", cmd.PetId);
            return UnitResult.Fail(Error.NotFound("Pet"));
        }

        pet.ChangePetStatus((HelpStatus)cmd.HelpStatus);

        var result = await _repository.Save(volunteer, ct);

        return result;
    }
}

