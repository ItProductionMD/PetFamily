using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;

public record SoftDeleteVolunteerCommand(Guid VolunteerId) : ICommand;
