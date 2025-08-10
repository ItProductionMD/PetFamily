using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Auth.Application.UserManagement.Commands.LoginUserByEmail;

public sealed record LoginByEmailCommand(string Email, string Password,string Fingerprint) : ICommand;

