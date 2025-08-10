using PetFamily.SharedApplication.Dtos;
using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.VolunteerRequests.Domain.Entities;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace PetFamily.VolunteerRequests.Application.Commands.UpdateVolunteerRequest;

public static class UpdateVolunteerRequestValidator
{
    public static void Validate(UpdateVolunteerRequestCommand cmd)
    {
        var result = UnitResult.FromValidationResults(
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

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }
}
