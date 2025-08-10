using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Volunteers.Application.Commands.VolunteerManagement.DeleteVolunteer;

public record HardDeleteVolunteerCommand(Guid VolunteerId) : ICommand;
