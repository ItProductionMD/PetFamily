using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Auth.Application.PermissionManagement.Commands.AddPermission;

public sealed record AddPermissionCommand(string Code) : ICommand;

