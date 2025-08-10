using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Auth.Application.UserManagement.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string EmailConfirmationToken) : ICommand;

