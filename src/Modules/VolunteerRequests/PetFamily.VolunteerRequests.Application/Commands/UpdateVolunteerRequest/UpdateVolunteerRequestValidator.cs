using PetFamily.SharedApplication.Dtos;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.VolunteerRequests.Domain.Entities;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace PetFamily.VolunteerRequests.Application.Commands.UpdateVolunteerRequest;

public static class UpdateVolunteerRequestValidator
{
    public static UnitResult Validate(UpdateVolunteerRequestCommand cmd)
    {
        return UnitResult.FromValidationResults(
            () => VolunteerRequest.Validate(
                Guid.NewGuid(),
                cmd.DocumentName,
                cmd.LastName,
                cmd.FirstName,
                cmd.Description,
                cmd.ExperienceYears),

            () => ValidateItems<RequisitesDto>(
                cmd.Requisites,
                r => RequisitesInfo.Validate(r.Name, r.Description)));
    }
}
