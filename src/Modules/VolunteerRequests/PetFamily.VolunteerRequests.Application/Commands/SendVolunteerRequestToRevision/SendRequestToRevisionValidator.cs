using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationConstants;

namespace PetFamily.VolunteerRequests.Application.Commands.SendVolunteerRequestToRevision;

public static class SendRequestToRevisionValidator
{
    public static UnitResult Validate(SendRequestToRevisionCommand cmd)
    {
        return UnitResult.FromValidationResults(
            () => ValidateIfGuidIsNotEpmty(cmd.VolunteerRequestId, "VolunteerRequestId"),
            () => ValidateRequiredField(cmd.Comment, "VolunteerRequest message", MAX_LENGTH_LONG_TEXT));
    }
}
