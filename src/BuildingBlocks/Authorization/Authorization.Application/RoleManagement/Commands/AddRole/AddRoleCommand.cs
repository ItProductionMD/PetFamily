
using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Authorization.Application.RoleManagement.Commands.AddRole;

public record AddRoleCommand(string RoleCode, IEnumerable<Guid> PermissionIds) : ICommand;

