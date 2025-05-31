using PetFamily.Application.Abstractions.CQRS;

namespace Volunteers.Application.Commands.VolunteerManagement.RestoreVolunteer;
public record RestoreVolunteerCommand(Guid VolunteerId) : ICommand;

