using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Auth.Application.UserManagement.Commands.ChangeRoles;

public record ChangeRoleCommand(Guid UserId, Guid RoleId) : ICommand;

