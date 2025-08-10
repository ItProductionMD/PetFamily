namespace PetFamily.Auth.Application.Dtos;

public record PermissionDto(Guid PermissionId, string PermissionCode, bool IsEnable);