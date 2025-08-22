using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Authorization.Application.RoleManagement.Commands.DeleteRole;

public record DeleteRoleCommand(Guid RoleId) : ICommand;

