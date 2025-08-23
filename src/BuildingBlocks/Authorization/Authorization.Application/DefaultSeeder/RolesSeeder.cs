using Authorization.Application.IRepositories.IAuthorizationRepo;
using Authorization.Domain.Entities;
using Authorization.Domain.Entities.RoleAggregate;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedKernel.Authorization;

namespace Authorization.Application.DefaultSeeder;

public class RolesSeeder(
    IRoleWriteRepo _roleWriteRepo,
    IRoleReadRepo _roleReadRepo,
    IPermissionWriteRepo _permissionWriteRepo,
    IPermissionReadRepo _permissionReadRepo,
    ILogger<RolesSeeder> logger):ISeeder
{
    
    private readonly ILogger<RolesSeeder> _logger = logger;
    private Role? _adminRole;
    private Role? _userRole;
    private Role? _volunteerRole;
    private Role? _unconfirmedUserRole;

    public async Task SeedAsync()
    {

        var existingRoles = await _roleReadRepo.GetRoles(default);
        if (existingRoles.Count > 0)
        {
            _logger.LogInformation("ROLE SEEDER: Roles already exist. Skipping seeding.");
            return;
        }

        var existingPermissions = await _permissionReadRepo.GetPermissionsAsync(default);
        if (existingPermissions.Count > 0)
        {
            _logger.LogInformation("ROLE SEEDER: Permissions already exist. Skipping seeding.");
            return;
        }

        var allPermissions = PermissionCodes.GetAllPermissionCodes()
            .Select(p => Permission.Create(p).Data!)
            .ToList();

        var permissionCodesForAdminRole = PermissionCodes.GetPermissionsForAdmin();
        var permissionCodesForUserRole = PermissionCodes.GetPermissionsForUser();
        var permissionCodesForVolunteerRole = PermissionCodes.GetPermissionsForVolunteer();
        var permissionCodesForUnconfirmedUser = PermissionCodes.GetPermissionsForUnconfirmedUser();
        try
        {
            await _permissionWriteRepo.AddPermissions(allPermissions, default);
            await _permissionWriteRepo.SaveAsync();
            var allPermissionsIds = allPermissions
                .Select(p => p.Id)
                .ToList();

            var permissionsIdsForAdminRole = allPermissions
                .Where(p => permissionCodesForAdminRole.Any(pC => pC == p.Code))
                .Select(p => p.Id)
                .ToList();

            var permissionIdsForUserRole = allPermissions
                .Where(p => permissionCodesForUserRole.Any(pV => pV == p.Code))
                .Select(p => p.Id)
                .ToList();

            var permissionIdsForUnconfirmedUserRole = allPermissions
                .Where(p => permissionCodesForUnconfirmedUser.Any(pU => pU == p.Code))
                .Select(p => p.Id)
                .ToList();

            var permissionIdsForVolunteerRole = allPermissions
                .Where(p => permissionCodesForVolunteerRole.Any(pC => pC == p.Code))
                .Select(p => p.Id)
                .ToList();

            _adminRole = Role.Create(
                Guid.NewGuid(),
                RoleCodes.ADMIN,
                []).Data!;

            _userRole = Role.Create(
                Guid.NewGuid(),
                RoleCodes.USER,
                []).Data!;

            _unconfirmedUserRole = Role.Create(
                Guid.NewGuid(),
                RoleCodes.UNCONFIRMED_USER,
                []).Data!;

            _volunteerRole = Role.Create(
                Guid.NewGuid(),
                RoleCodes.VOLUNTEER,
                []).Data!;

            await _roleWriteRepo.AddRoles([
                _adminRole,
                _userRole,
                _unconfirmedUserRole ,
                _volunteerRole],
                default);


            _adminRole.UpdatePermissions(permissionsIdsForAdminRole);
            _userRole.UpdatePermissions(permissionIdsForUserRole);
            _unconfirmedUserRole.UpdatePermissions(permissionIdsForUserRole);
            _volunteerRole.UpdatePermissions(permissionIdsForVolunteerRole);

            await _roleWriteRepo.SaveChangesAsync();
            _logger.LogInformation("ROLE SEEDER: Roles seeding completed with Admin: {AdminRole}, User: {UserRole}," +
                " Volunteer: {VolunteerRole}, UnconfirmedUser: {UnconfirmedUserRole}",
                _adminRole.Code, _userRole.Code, _volunteerRole.Code, _unconfirmedUserRole.Code);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ROLE SEEDER: Error while seeding roles. Performing rollback.");

            throw;
        }
    }
}

