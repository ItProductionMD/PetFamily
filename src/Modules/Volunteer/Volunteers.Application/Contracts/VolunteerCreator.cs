using PetFamily.SharedApplication.Dtos;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;
using Volunteers.Public.Dto;
using Volunteers.Public.IContracts;

namespace Volunteers.Application.Contracts;

public class VolunteerCreator(
    CreateVolunteerHandler handler) : IVolunteerCreator
{
    public async Task<Result<Guid>> CreateVolunteer(
        CreateVolunteerDto dto,
        CancellationToken ct = default)
    {
        var cmd = new CreateVolunteerCommand(
            dto.FirstName,
            dto.LastName,
            "",
            dto.ExperienceYears,
            dto.Requisites.Select(r=>new RequisitesDto(r.Name,r.Description)));

        var result = await handler.Handle(cmd, ct);

        return result;
    }
}
