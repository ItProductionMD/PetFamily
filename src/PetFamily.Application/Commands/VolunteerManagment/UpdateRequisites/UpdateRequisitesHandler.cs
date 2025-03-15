using Microsoft.Extensions.Options;
using PetFamily.Domain.Results;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using PetFamily.Domain.Shared.ValueObjects;
using Microsoft.Extensions.Logging;
using PetFamily.Application.IRepositories;
using PetFamily.Application.Commands.PetManagment.Dtos;

namespace PetFamily.Application.Commands.VolunteerManagment.UpdateRequisites;

public class UpdateRequisitesHandler(
    IVolunteerRepository repository,
    ILogger<UpdateRequisitesHandler> logger)
{
    private readonly IVolunteerRepository _volunteerRepository = repository;
    private readonly ILogger<UpdateRequisitesHandler> _logger = logger;
    public async Task<UnitResult> Handle(
        Guid volunteerId,
        IEnumerable<RequisitesDto> dtos,
        CancellationToken cancellationToken = default)
    {
        var validateRequisites = ValidateItems(
            dtos, r => RequisitesInfo.Validate(r.Name, r.Description));
        if (validateRequisites.IsFailure)
        {
            _logger.LogWarning("Validate Requisites for volunteer failure!Errors:{Errors}",
                validateRequisites.ToErrorMessages());

            return validateRequisites;
        }
        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);

        var requisites = dtos.Select(c => RequisitesInfo.Create(c.Name, c.Description).Data!);

        volunteer.UpdateRequisites(requisites);

        await _volunteerRepository.Save(volunteer, cancellationToken);

        _logger.LogInformation("Update requisites for volunteer with id:{Id} successful!",
            volunteerId);

        return UnitResult.Ok();
    }
}
