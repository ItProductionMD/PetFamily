using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Auth.Application.RoleManagement.Commands.AddRole;

public record AddRoleCommand(string RoleCode, IEnumerable<Guid> PermissionIds) : ICommand;
