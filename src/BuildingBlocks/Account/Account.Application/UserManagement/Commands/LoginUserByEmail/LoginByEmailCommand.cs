using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Account.Application.UserManagement.Commands.LoginUserByEmail;

public sealed record LoginByEmailCommand(string Email, string Password, string Fingerprint) : ICommand;

