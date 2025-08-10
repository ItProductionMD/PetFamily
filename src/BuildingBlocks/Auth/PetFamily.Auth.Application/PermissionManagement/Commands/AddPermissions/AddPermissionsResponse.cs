using PetFamily.Auth.Application.Dtos;

namespace PetFamily.Auth.Application.PermissionManagement.Commands.AddPermissions;

public class AddPermissionsResponse
{
    public List<PermissionDtoWithError> PermissionDtoWithErrors { get; set; } = [];
    public List<PermissionDto> AddedPermissions { get; set; } = [];
}
