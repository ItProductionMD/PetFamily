using PetFamily.Application.Abstractions.CQRS;
using Volunteers.Application.ResponseDtos;

namespace Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

public record CreateVolunteerCommand(
    string FirstName,
    string LastName,
    string Email,
    string Description,
    string PhoneNumber,
    string PhoneRegionCode,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites,
    IEnumerable<SocialNetworksDto> SocialNetworksList) : ICommand;