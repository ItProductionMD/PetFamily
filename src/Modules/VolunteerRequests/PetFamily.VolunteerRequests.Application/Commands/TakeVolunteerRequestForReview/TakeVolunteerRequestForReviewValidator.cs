using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.VolunteerRequests.Application.Commands.TakeVolunteerRequestForReview;

public static class TakeVolunteerRequestForReviewValidator
{
    public static UnitResult Validate(TakeVolunteerRequestForReviewCommand command)
    {
        if (command.VolunteerRequestId == Guid.Empty)
        {
            return Result.Fail(Error.GuidIsEmpty("VolunteerRequestId"));
        }
        return UnitResult.Ok();
    }
}
