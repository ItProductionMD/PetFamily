using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Application.IRepositories;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateRequisites;

public class UpdateRequisitesHandler(
    IVolunteerWriteRepository repository,
    ILogger<UpdateRequisitesHandler> logger) : ICommandHandler<UpdateRequisitesCommand>
{
    private readonly IVolunteerWriteRepository _repository = repository;
    private readonly ILogger<UpdateRequisitesHandler> _logger = logger;
    public async Task<UnitResult> Handle(
        UpdateRequisitesCommand cmd,
        CancellationToken ct = default)
    {
        var validateRequisites = ValidateItems(
            cmd.RequisitesDtos, r => RequisitesInfo.Validate(r.Name, r.Description));
        if (validateRequisites.IsFailure)
        {
            _logger.LogWarning("Validate Requisites for volunteer failure!Errors:{Errors}",
                validateRequisites.ValidationMessagesToString());

            return validateRequisites;
        }
        var getVolunteer = await _repository.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var requisites = cmd.RequisitesDtos.Select(c =>
            RequisitesInfo.Create(c.Name, c.Description).Data!);

        volunteer.UpdateRequisites(requisites);

        await _repository.Save(volunteer, ct);

        _logger.LogInformation("Update requisites for volunteer with id:{Id} successful!",
            cmd.VolunteerId);

        return UnitResult.Ok();
    }
}
