using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;
using Volunteers.Domain.Enums;

namespace Volunteers.Application.Commands.PetManagement.UpdatePetStatus;

public class UpdatePetStatusHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    ILogger<UpdatePetStatusHandler> logger) : ICommandHandler<UpdatePetStatusCommand>
{
    public async Task<UnitResult> Handle(UpdatePetStatusCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var getVolunteer = await volunteerWriteRepo.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == cmd.PetId);
        if (pet == null)
        {
            logger.LogWarning("Pet with id:{Id} not found!", cmd.PetId);
            return UnitResult.Fail(Error.NotFound("Pet"));
        }

        pet.ChangePetStatus((HelpStatus)cmd.HelpStatus);

        var result = await volunteerWriteRepo.SaveAsync(volunteer, ct);

        return result;
    }
}

