using PetFamily.Application.Abstractions.CQRS;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;

public record UpdateVolunteerCommand
(
    Guid VolunteerId,
    string FirstName,
    string LastName,
    string Email,
    string Description,
    string PhoneNumber,
    string PhoneRegionCode,
    int ExperienceYears
) : ICommand;
