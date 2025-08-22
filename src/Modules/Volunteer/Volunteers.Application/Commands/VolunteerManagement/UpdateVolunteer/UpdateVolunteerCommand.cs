using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;

public record UpdateVolunteerCommand
(
    Guid UserId,
    Guid VolunteerId,
    string FirstName,
    string LastName,
    string Description,
    int ExperienceYears
) : ICommand;
