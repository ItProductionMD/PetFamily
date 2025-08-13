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
            dto.AdminId,
            dto.UserId,
            dto.FirstName,
            dto.LastName,
            "",
            dto.ExperienceYears,
            "+39",
            "000000",
            dto.Requisites.Select(r=>new RequisitesDto(r.Name,r.Description)));

        var result = await handler.Handle(cmd, ct);

        return result;
    }
}
