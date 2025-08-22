using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Authorization.Application.PermissionManagement.Commands.AddPermissions;

public record AddPermissionsCommand(List<string> newPermissionCodes) : ICommand;

