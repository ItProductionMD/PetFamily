using PetFamily.Application.Abstractions;
using PetFamily.Application.Dtos;

namespace PetFamily.Application.Commands.VolunteerManagment.UpdateRequisites;

public record class UpdateRequisitesCommand(
    Guid VolunteerId,
    IEnumerable<RequisitesDto> RequisitesDtos) : ICommand;

