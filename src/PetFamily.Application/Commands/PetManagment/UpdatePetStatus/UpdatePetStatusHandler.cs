using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Enums;
using PetFamily.Domain.Results;
using static PetFamily.Application.Commands.PetManagment.UpdatePetStatus.UpdatePetStatusValidator;

namespace PetFamily.Application.Commands.PetManagment.UpdatePetStatus;

public class UpdatePetStatusHandler(
    ILogger<UpdatePetStatusHandler> logger,
    IVolunteerWriteRepository volunteerWriteRepository) : ICommandHandler<UpdatePetStatusCommand>
{
    private readonly IVolunteerWriteRepository _writeRepository = volunteerWriteRepository;
    private readonly ILogger<UpdatePetStatusHandler> _logger = logger;

    public async Task<UnitResult> Handle(UpdatePetStatusCommand cmd, CancellationToken cancelToken)
    {
        var validate = Validate(cmd);
        if (validate.IsFailure)
        {
            _logger.LogWarning("Validate updatePetStatusCommand for volunteer with id:{volunteerId} " +
                "and pet with id:{petId}error:{Errors}",
                cmd.VolunteerId, cmd.PetId, validate.ValidationMessagesToString());

            return validate;
        }

        var getVolunteer = await _writeRepository.GetByIdAsync(cmd.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == cmd.PetId);
        if (pet == null)
        {
            _logger.LogWarning("Pet with id:{Id} not found!", cmd.PetId);
            return UnitResult.Fail(Error.NotFound("Pet"));
        }

        pet.UpdateHelpStatus((HelpStatus)cmd.HelpStatus);

        var result = await _writeRepository.Save(volunteer, cancelToken);

        return result;
    }
}

