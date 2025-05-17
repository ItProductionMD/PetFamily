using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;

namespace PetFamily.Application.Commands.VolunteerManagment.UpdateRequisites;

public class UpdateRequisitesHandler(
    IVolunteerWriteRepository repository,
    ILogger<UpdateRequisitesHandler> logger) : ICommandHandler<UpdateRequisitesCommand>
{
    private readonly IVolunteerWriteRepository _volunteerRepository = repository;
    private readonly ILogger<UpdateRequisitesHandler> _logger = logger;
    public async Task<UnitResult> Handle(
        UpdateRequisitesCommand command,
        CancellationToken cancelToken = default)
    {
        var validateRequisites = ValidateItems(
            command.RequisitesDtos, r => RequisitesInfo.Validate(r.Name, r.Description));
        if (validateRequisites.IsFailure)
        {
            _logger.LogWarning("Validate Requisites for volunteer failure!Errors:{Errors}",
                validateRequisites.ValidationMessagesToString());

            return validateRequisites;
        }
        var getVolunteer = await _volunteerRepository.GetByIdAsync(command.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var requisites = command.RequisitesDtos.Select(c =>
            RequisitesInfo.Create(c.Name, c.Description).Data!);

        volunteer.UpdateRequisites(requisites);

        await _volunteerRepository.Save(volunteer, cancelToken);

        _logger.LogInformation("Update requisites for volunteer with id:{Id} successful!",
            command.VolunteerId);

        return UnitResult.Ok();
    }
}
