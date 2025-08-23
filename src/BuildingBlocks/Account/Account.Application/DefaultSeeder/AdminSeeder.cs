using Account.Application.IRepositories;
using Account.Application.IServices;
using Account.Application.Options;
using Account.Domain.Entities.UserAggregate;
using Account.Domain.Enums;
using Account.Domain.Options;
using Authorization.Public.Contracts;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using static Account.Domain.Validations.Validations;

namespace Account.Application.DefaultSeeder;

public class AdminSeeder(
    IAdminAuthorizationCreator adminAuthorization,
    AdminIdentity adminOptions,
    IPasswordHasher _passwordHasher,
    IUserWriteRepository _userWriteRepository,
    IUserReadRepository _userReadRepository,
    ILogger<AdminSeeder> logger): ISeeder
{
    private readonly AdminIdentity _adminOptions = adminOptions;

    public async Task SeedAsync()
    {
        var validateResult = UnitResult.FromValidationResults(
            () => ValidateEmail(_adminOptions.Email),
            () => ValidateLogin(_adminOptions.Login, LoginOptions.Default),
            () => ValidatePassword(_adminOptions.Password));

        if (validateResult.IsFailure)
        {
            logger.LogWarning("ADMIN SEEDER: validate admin  fail! Errors:{errors}",
                validateResult.ValidationMessagesToString());

            throw new InvalidOperationException("Admin seeding failed due to validation errors!");
        }
        var checkAdminUniqness = await _userReadRepository.CheckUniqueFields(
            _adminOptions.Email, _adminOptions.Login, Phone.CreateEmpty(), default);
        if (checkAdminUniqness.IsFailure)
        {
            logger.LogInformation("ADMIN SEEDER: Admin user with email {Email} or login {Login} already exists. " +
                "Skipping seeding.", _adminOptions.Email, _adminOptions.Login);
            return;
        }

        var hashedPassword = _passwordHasher.Hash(_adminOptions.Password);

        var userResult = User.Create(
            UserId.NewGuid(),
            _adminOptions.Login,
            _adminOptions.Email,
            Phone.CreateEmpty(),
            hashedPassword,
            [],
            ProviderType.Local);

        if (userResult.IsFailure)
        {
            logger.LogCritical("ADMIN SEEDER: Failed to create admin user: {Error}", userResult.Error);
            throw new InvalidOperationException($"Failed to create admin user: {userResult.Error}");
        }
        var adminUser = userResult.Data!;

        await _userWriteRepository.AddAndSaveAsync(adminUser, default);

        logger.LogInformation("ADMIN SEEDER: Admin user with email *** and login *** created successfully.");

        try
        {
            await adminAuthorization.CreateAdminAuthorization(adminUser.Id.Value, default);

            logger.LogInformation("ADMIN SEEDER: Admin Authorization  was created successful!");
        }
        catch (Exception ex)
        {
            logger.LogError("ADMIN SEEDER: Error Create Authorization for admin! Exception:{Message}", ex.Message);
        }
    }

}
