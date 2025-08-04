using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Errors;
using PetFamily.VolunteerRequests.Domain.Enums;

namespace PetFamily.VolunteerRequests.Application.Queries.GetRequestsOnReview;

public static class GetRequestsOnReviewValidator
{
    public static void Validate(GetRequestsOnReviewQuery query)
    {
        foreach (var item in query.Filter.Statuses)
        {
            if (Enum.TryParse<RequestStatus>(item, true, out var result) == false)
                throw new ValidationException(Error.InvalidFormat("status"));
        }
    }
}
