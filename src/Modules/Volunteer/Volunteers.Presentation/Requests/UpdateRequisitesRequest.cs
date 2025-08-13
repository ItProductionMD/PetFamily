using PetFamily.SharedApplication.Dtos;
using Volunteers.Application.Commands.VolunteerManagement.UpdateRequisites;

namespace Volunteers.Presentation.Requests;

public class UpdateRequisitesRequest
{
    public IEnumerable<RequisitesDto> RequisitesDtos { get; set; } = [];

    public UpdateRequisitesCommand ToCommand(Guid userId, Guid volunteerId)
    {
        return new UpdateRequisitesCommand(userId, volunteerId, RequisitesDtos);
    }
}
