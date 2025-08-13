using PetFamily.SharedApplication.Dtos;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;
using PetFamily.VolunteerRequests.Public.Contracts;
using PetFamily.VolunteerRequests.Public.Dtos;

namespace PetFamily.VolunteerRequests.Application.ContractsImplementation;

public class VolunteerRequestSubmission(SubmitVolunteerRequestHandler handler)
    : IVolunteerRequestSubmission
{
    public Task<UnitResult> SubmitVolunteerRequestAsync(
        SubmitVolunteerRequestDto dto,
        CancellationToken ct)
    {
        var cmd = new SubmitVolunteerRequestCommand(
            dto.userId,
            dto.documentName,
            dto.lastName,
            dto.firstName,
            dto.description,
            dto.experienceYears,
            dto.requisites.Select(r => new RequisitesDto(r.Name, r.Description)).ToList());

        return handler.Handle(cmd, ct);
    }
}
