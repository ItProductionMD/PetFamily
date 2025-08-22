using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateRequisites;

public record class UpdateRequisitesCommand(
    Guid UserId,
    Guid VolunteerId,
    IEnumerable<RequisitesDto> RequisitesDtos) : ICommand;

