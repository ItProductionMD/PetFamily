namespace Account.Application.Dtos;

public record RoleDto(Guid RoleId, string RoleCode, IEnumerable<PermissionDto> Permissions);
