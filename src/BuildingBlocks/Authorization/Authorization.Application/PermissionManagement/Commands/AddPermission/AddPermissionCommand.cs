using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Authorization.Application.PermissionManagement.Commands.AddPermission;

public sealed record AddPermissionCommand(string Code) : ICommand;

