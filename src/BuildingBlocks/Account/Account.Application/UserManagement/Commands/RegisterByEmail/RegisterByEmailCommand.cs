using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;

namespace Account.Application.UserManagement.Commands.RegisterByEmail;

public record RegisterByEmailCommand(
    string Email,
    string Login,
    string Password,
    string phoneRegionCode,
    string phoneNumber,
    IEnumerable<SocialNetworksDto> SocialNetworksList) : ICommand;

