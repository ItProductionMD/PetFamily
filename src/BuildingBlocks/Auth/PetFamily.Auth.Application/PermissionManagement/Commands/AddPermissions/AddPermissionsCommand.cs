using PetFamily.Application.Abstractions.CQRS;

namespace PetFamily.Auth.Application.PermissionManagement.Commands.AddPermissions;

public record AddPermissionsCommand(List<string> newPermissionCodes) : ICommand;
