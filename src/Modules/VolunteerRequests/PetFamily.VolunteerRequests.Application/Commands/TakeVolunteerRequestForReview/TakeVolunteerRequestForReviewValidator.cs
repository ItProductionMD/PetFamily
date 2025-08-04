using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.VolunteerRequests.Application.Commands.TakeVolunteerRequestForReview;

public static class TakeVolunteerRequestForReviewValidator
{
    public static void Validate(TakeVolunteerRequestForReviewCommand command)
    {
        if (command.VolunteerRequestId == Guid.Empty)
            throw new ValidationException(Error.GuidIsEmpty("VolunteerRequestId"));
        
    }
}
