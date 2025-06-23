namespace PetFamily.Auth.Application.Dtos;

public record RoleDto(Guid RoleId, string RoleCode, IEnumerable<PermissionDto> Permissions);
