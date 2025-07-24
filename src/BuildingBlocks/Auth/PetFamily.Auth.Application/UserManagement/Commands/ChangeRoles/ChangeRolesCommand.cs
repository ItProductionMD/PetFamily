using PetFamily.Application.Abstractions.CQRS;

namespace PetFamily.Auth.Application.UserManagement.Commands.ChangeRoles;

public record ChangeRolesCommand(Guid UserId, IEnumerable<Guid> RoleIds) : ICommand;

