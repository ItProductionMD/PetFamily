using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Authorization.Application.RoleManagement.Commands.AddRoles;

public record AddRolesCommand(List<RequestRoleDto> roleDtos) : ICommand;

public record RequestRoleDto(string RoleName, string RoleCode, IEnumerable<Guid> PermissionsIds);
