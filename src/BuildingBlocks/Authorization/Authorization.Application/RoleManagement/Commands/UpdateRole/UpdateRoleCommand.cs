using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Authorization.Application.RoleManagement.Commands.UpdateRole;
public record UpdateRoleCommand(
    Guid RoleId,
    IEnumerable<Guid> PermissionsId) : ICommand;