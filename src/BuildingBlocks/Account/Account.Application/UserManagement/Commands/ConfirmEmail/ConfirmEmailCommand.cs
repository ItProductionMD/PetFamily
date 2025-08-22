using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Account.Application.UserManagement.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string EmailConfirmationToken) : ICommand;

