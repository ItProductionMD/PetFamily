using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;

namespace Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

public record CreateVolunteerCommand(
    Guid AdminId,
    Guid UserId,
    string FirstName,
    string LastName,
    string Description,
    int ExperienceYears,
    string PhoneRegionCode,
    string PhoneNumber,
    IEnumerable<RequisitesDto> Requisites) : ICommand;