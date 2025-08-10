using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;

public record UpdateVolunteerCommand
(
    Guid VolunteerId,
    string FirstName,
    string LastName, 
    string Description,
    int ExperienceYears
) : ICommand;
