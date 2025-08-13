using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Auth.Application.UserManagement.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string Token, string FingerPrint) : ICommand;


