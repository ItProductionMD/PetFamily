
using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;

public record HardDeleteVolunteerCommand(Guid VolunteerId) : ICommand;
