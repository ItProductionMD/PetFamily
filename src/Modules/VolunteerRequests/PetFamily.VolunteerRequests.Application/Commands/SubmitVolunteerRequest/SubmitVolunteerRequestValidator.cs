using PetFamily.SharedApplication.Dtos;
using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.SharedKernel.ValueObjects;


namespace PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;

public static class SubmitVolunteerRequestValidator
{
    public static UnitResult Validate(SubmitVolunteerRequestCommand cmd)
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
