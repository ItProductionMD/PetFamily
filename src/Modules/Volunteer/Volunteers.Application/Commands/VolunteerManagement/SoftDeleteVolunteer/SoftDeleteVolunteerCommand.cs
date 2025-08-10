using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Volunteers.Application.Commands.VolunteerManagement.SoftDeleteVolunteer;

public record SoftDeleteVolunteerCommand(Guid VolunteerId) : ICommand;
