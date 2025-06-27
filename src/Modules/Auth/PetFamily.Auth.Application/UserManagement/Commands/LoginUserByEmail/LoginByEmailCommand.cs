using PetFamily.Application.Abstractions.CQRS;

namespace PetFamily.Auth.Application.UserManagement.Commands.LoginUserByEmail;

public sealed record LoginByEmailCommand(string Email, string Password,string Fingerprint) : ICommand;

