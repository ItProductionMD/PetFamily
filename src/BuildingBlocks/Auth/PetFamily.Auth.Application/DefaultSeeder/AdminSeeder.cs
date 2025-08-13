using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Auth.Application.AdminOptions;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.Auth.Domain.Enums;
using PetFamily.Auth.Domain.Options;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Authorization;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using static PetFamily.Auth.Domain.Validations.Validations;

namespace PetFamily.Auth.Application.DefaultSeeder;

public class AdminSeeder(
    IOptions<AdminIdentity> adminOptions,
    ILogger<AdminSeeder> logger,
    IPasswordHasher passwordHasher,
    IRoleReadRepository roleReadRepository,
    IUserWriteRepository userWriteRepository,
    IUserReadRepository userReadRepository)
{
    private readonly AdminIdentity _admin = adminOptions.Value;
    private readonly ILogger<AdminSeeder> _logger = logger;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IRoleReadRepository _roleReadRepository = roleReadRepository;
    private readonly IUserWriteRepository _userWriteRepository = userWriteRepository;
    private readonly IUserReadRepository _userReadRepository = userReadRepository;

    public async Task SeedAsync()
    {
        var validateResult = UnitResult.FromValidationResults(
            () => ValidateEmail(_admin.Email),
            () => ValidateLogin(_admin.Login, LoginOptions.Default),
            () => ValidatePassword(_admin.Password));

        if (validateResult.IsFailure)
        {
            _logger.LogWarning("ADMIN SEEDER: validate admin  fail! Errors:{errors}",
                validateResult.ValidationMessagesToString());

            throw new InvalidOperationException("Admin seeding failed due to validation errors!");
        }
        var checkAdminUniqness = await _userReadRepository.CheckUniqueFields(
            _admin.Email, _admin.Login, Phone.CreateEmpty(), default);
        if (checkAdminUniqness.IsFailure)
        {
            _logger.LogInformation("ADMIN SEEDER: Admin user with email {Email} or login {Login} already exists. " +
                "Skipping seeding.", _admin.Email, _admin.Login);
            return;
        }

        var getAdminRole = await _roleReadRepository.GetByCodeAsync(RoleCodes.ADMIN, default);
        if (getAdminRole.IsFailure)
        {
            _logger.LogCritical("ADMIN SEEDER: Admin role does not exist!");
            throw new InvalidOperationException("Admin role does not exist." +
                " Please create it before seeding the admin user.");
        }
        var guidRoleId = getAdminRole.Data!.RoleId;
        var adminRoleId = RoleId.Create(guidRoleId).Data!;

        var hashedPassword = _passwordHasher.Hash(_admin.Password);

        var userResult = User.Create(
            UserId.NewGuid(),
            _admin.Login,
            _admin.Email,
            Phone.CreateEmpty(),
            hashedPassword,
            [],
            adminRoleId,
            ProviderType.Local);

        if (userResult.IsFailure)
        {
            _logger.LogCritical("ADMIN SEEDER: Failed to create admin user: {Error}", userResult.Error);
            throw new InvalidOperationException($"Failed to create admin user: {userResult.Error}");
        }
        var adminUser = userResult.Data!;

        await _userWriteRepository.AddAsync(adminUser, default);

        await _userWriteRepository.SaveChangesAsync(default);

        _logger.LogInformation("ADMIN SEEDER: Admin user with email {Email} and login {Login} created successfully.",
            _admin.Email, _admin.Login);
    }

}
