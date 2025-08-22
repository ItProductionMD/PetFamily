
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;

namespace Account.Application.UserManagement.Commands.UpdateUser;

public record UpdateUserProfileCommand(
    Guid UserId,
    bool HasVolunteerAccount,
    string NewLogin,
    string NewPhoneRegionCode,
    string NewPhoneNumber,
    List<SocialNetworksDto> NewSocialNetworks) : ICommand;

