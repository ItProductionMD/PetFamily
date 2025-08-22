using PetFamily.SharedApplication.Abstractions.CQRS;
using System.Security.Claims;

namespace Authorization.Application.RefreshTokenSessionManagement.Commands.RefreshTokens;

public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken,
    string FingerPrint) : ICommand;
