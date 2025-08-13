
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;

namespace PetFamily.Auth.Application.UserManagement.Commands.UpdateUser;

public record UpdateUserProfileCommand(
    string NewLogin,
    string NewPhoneRegionCode,
    string NewPhoneNumber,
    List<SocialNetworksDto> NewSocialNetworks) : ICommand;

