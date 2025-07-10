using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Public.Dtos;

namespace PetFamily.VolunteerRequests.Public.Contracts;

public interface IVolunteerRequestSubmission
{
    Task<UnitResult> SubmitVolunteerRequestAsync(SubmitVolunteerRequestDto submitVolunteerRequestDto);
}
