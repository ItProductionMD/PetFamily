using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Auth.Domain.ValueObjects;

namespace PetFamily.Auth.Application.RoleManagement.Commands.UpdateRole;

public record UpdateRoleCommand(
    RoleId RoleId,
    IEnumerable<Guid> PermissionsId) : ICommand;
