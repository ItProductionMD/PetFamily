using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Auth.Application.RoleManagement.Commands.UpdateRole;

public record UpdateRoleCommand(
    RoleId RoleId,
    IEnumerable<Guid> PermissionsId) : ICommand;
