using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Application.Volunteers.UpdateVolunteer;

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
);
