using PetFamily.Application.Abstractions.CQRS;

namespace PetFamily.Auth.Application.RoleManagement.Commands.DeleteRole;

public record DeleteRoleCommand(Guid RoleId) : ICommand;

