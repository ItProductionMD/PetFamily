using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;

namespace PetFamily.Auth.Application.UserManagement.Commands.RegisterByEmail;

public record RegisterByEmailCommand(
    string Email,
    string Login,
    string Password,
    string phoneRegionCode,
    string phoneNumber,
    IEnumerable<SocialNetworksDto> SocialNetworksList) : ICommand;

