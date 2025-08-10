using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Auth.Application.PermissionManagement.Commands.AddPermissions;

public record AddPermissionsCommand(List<string> newPermissionCodes) : ICommand;
