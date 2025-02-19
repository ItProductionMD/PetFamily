using Microsoft.Extensions.Options;
using PetFamily.Domain.Results;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using PetFamily.Domain.Shared.ValueObjects;
using Microsoft.Extensions.Logging;
using static PetFamily.Application.Volunteers.SharedVolunteerRequests;

namespace PetFamily.Application.Volunteers.UpdateRequisites;

public class UpdateRequisitesHandler(
    IVolunteerRepository repository,
    ILogger<UpdateRequisitesHandler> logger)
{
    private readonly IVolunteerRepository _volunteerRepository = repository;
    private readonly ILogger<UpdateRequisitesHandler> _logger = logger;
    public async Task<UnitResult> Handle(
        Guid volunteerId,
        IEnumerable<RequisitesRequest> dtos,
        CancellationToken cancellationToken = default)
    {
        var validateRequisites = ValidateItems(
            dtos, r => RequisitesInfo.Validate(r.Name, r.Description));
        if (validateRequisites.IsFailure)
        {
            _logger.LogWarning("Validate Requisites for volunteer failure!Errors:{Errors}",
                validateRequisites.ConcateErrorMessages());

            return validateRequisites;
        }
        var getVolunteer = await _volunteerRepository.GetById(volunteerId, cancellationToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Fail get volunteer with Id:{volunteerId} for update requisites!Errors:{Errors}",
                volunteerId, getVolunteer.ConcateErrorMessages());

            return UnitResult.Fail(getVolunteer.Errors!);
        }
        var volunteer = getVolunteer.Data!;

        var requisites = dtos.Select(c => RequisitesInfo.Create(c.Name, c.Description).Data!);

        volunteer.UpdateRequisites(requisites);

        await _volunteerRepository.Save(volunteer, cancellationToken);

        _logger.LogInformation("Update requisites for volunteer with id:{Id} successful!",
            volunteerId);

        return UnitResult.Ok();
    }
}
