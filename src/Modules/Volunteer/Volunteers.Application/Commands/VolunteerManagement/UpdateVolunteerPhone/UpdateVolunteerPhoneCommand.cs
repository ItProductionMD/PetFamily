using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteerPhone;

public record UpdateVolunteerPhoneCommand(
    Guid UserId,
    string PhoneRegionCode,
    string PhoneNumber) : ICommand;

