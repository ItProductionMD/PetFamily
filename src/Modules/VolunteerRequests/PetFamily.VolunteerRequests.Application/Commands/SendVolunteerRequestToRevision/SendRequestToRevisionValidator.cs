using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace PetFamily.VolunteerRequests.Application.Commands.SendVolunteerRequestToRevision;

public static class SendRequestToRevisionValidator
{
    public static void Validate(this SendRequestToRevisionCommand cmd)
    {
        var result = UnitResult.FromValidationResults(
            () => ValidateIfGuidIsNotEpmty(cmd.VolunteerRequestId, "VolunteerRequestId"),
            () => ValidateRequiredField(cmd.Comment, "VolunteerRequest message", MAX_LENGTH_LONG_TEXT));

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }
}
