using Microsoft.Extensions.Logging;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.Entities;
using PetFamily.Auth.Domain.Entities.RoleAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Authorization;

namespace PetFamily.Auth.Application.DefaultSeeder;

public class RolesSeeder(
    IRoleWriteRepository roleWriteRepository,
    IRoleReadRepository roleReadRepository,
    IPermissionWriteRepository permissionWriteRepository,
    IPermissionReadRepository permissionReadRepository,
    IAuthUnitOfWork authUnitOfWork,
    ILogger<RolesSeeder> logger)
{
    private readonly IRoleWriteRepository _roleWriteRepository = roleWriteRepository;
    private readonly IRoleReadRepository _roleReadRepository = roleReadRepository;
    private readonly IPermissionWriteRepository _permissionWriteRepository = permissionWriteRepository;
    private readonly IPermissionReadRepository _permissionReadRepository = permissionReadRepository;
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork;
    private readonly ILogger<RolesSeeder> _logger = logger;
    private Role? _adminRole;
    private Role? _userRole;
    private Role? _volunteerRole;
    private Role? _unconfirmedUserRole;

    public async Task SeedAsync()
    {

        var result = await _roleReadRepository.GetRoles(default);
        if (result.Count > 0)
        {
            _logger.LogInformation("ROLE SEEDER: Roles already exist. Skipping seeding.");
            return;
        }

        var permissionResult = await _permissionReadRepository.GetPermissionsAsync(default);
        if (permissionResult.Count > 0)
        {
            _logger.LogInformation("ROLE SEEDER: Permissions already exist. Skipping seeding.");
            return;
        }

        var allPermissions = PermissionCodes.GetAllPermissionCodes()
            .Select(p => Permission.Create(p).Data!)
            .ToList();

        List<string> permissionCodesForUserRole = PermissionCodes.UserManagement
            .GetPermissions();

        List<string> permissionCodesForVolunteerRole = PermissionCodes.VolunteerManagement
            .GetPermissions();
        permissionCodesForVolunteerRole.AddRange(permissionCodesForUserRole);

        List<string> permissionCodesForUnconfirmedUser = [
            PermissionCodes.UserManagement.UserView];

        await _authUnitOfWork.BeginTransactionAsync(default);
        try
        {
            await _permissionWriteRepository.AddPermissions(allPermissions, default);

            await _authUnitOfWork.SaveChangesAsync(default);

            var authPermissionsIds = allPermissions
                .Select(p => p.Id)
                .ToList();

            var userPermissionIds = allPermissions
                .Where(p => permissionCodesForUserRole.Any(pU => pU == p.Code))
                .Select(p => p.Id)
                .ToList();

            var volunteerPermissionIds = allPermissions
                .Where(p => permissionCodesForVolunteerRole.Any(pV => pV == p.Code))
                .Select(p => p.Id)
                .ToList();

            var unconfirmedUserPermissionIds = allPermissions
                .Where(p => permissionCodesForUnconfirmedUser.Any(pU => pU == p.Code))
                .Select(p => p.Id)
                .ToList();

            _adminRole = Role.Create(
                RoleId.New(),
                RoleCodes.ADMIN,
                []).Data!;

            _userRole = Role.Create(
                RoleId.New(),
                RoleCodes.USER,
                []).Data!;

            _unconfirmedUserRole = Role.Create(
                RoleId.New(),
                RoleCodes.UNCONFIRMED_USER,
                []).Data!;

            _volunteerRole = Role.Create(
                RoleId.New(),
                RoleCodes.VOLUNTEER,
                []).Data!;

            await _roleWriteRepository.AddRoles([
                _adminRole,
                _userRole,
                _unconfirmedUserRole ,
                _volunteerRole],
                default);

            await _authUnitOfWork.SaveChangesAsync(default);

            _adminRole.Update(authPermissionsIds);
            _userRole.Update(userPermissionIds);
            _unconfirmedUserRole.UpdatePermissions(unconfirmedUserPermissionIds);
            _volunteerRole.UpdatePermissions(volunteerPermissionIds);

            await _authUnitOfWork.SaveChangesAsync(default);

            _logger.LogInformation("ROLE SEEDER: Roles seeding completed with Admin: {AdminRole}, User: {UserRole}," +
                " Volunteer: {VolunteerRole}, UnconfirmedUser: {UnconfirmedUserRole}",
                _adminRole.Code, _userRole.Code, _volunteerRole.Code, _unconfirmedUserRole.Code);

            await _authUnitOfWork.CommitAsync(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ROLE SEEDER: Error while seeding roles. Performing rollback.");

            await _authUnitOfWork.RollbackAsync();

            throw;
        }
    }
}
