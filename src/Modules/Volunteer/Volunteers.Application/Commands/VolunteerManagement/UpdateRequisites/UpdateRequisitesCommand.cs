using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;
using Volunteers.Application.ResponseDtos;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateRequisites;

public record class UpdateRequisitesCommand(
    Guid UserId,
    Guid VolunteerId,
    IEnumerable<RequisitesDto> RequisitesDtos) : ICommand;

