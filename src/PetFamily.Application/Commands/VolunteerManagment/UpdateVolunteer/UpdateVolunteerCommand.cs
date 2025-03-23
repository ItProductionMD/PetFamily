using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.VolunteerManagment.UpdateVolunteer;

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
