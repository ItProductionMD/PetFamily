using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Auth.Application;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Infrastructure.BackgroundServices;
using PetFamily.Auth.Infrastructure.Contexts;
using PetFamily.Auth.Infrastructure.Dapper;
using PetFamily.Auth.Infrastructure.Repository;
using PetFamily.Auth.Infrastructure.Services.AuthorizationService;
using PetFamily.Auth.Infrastructure.Services.EmailService;
using PetFamily.Auth.Infrastructure.Services.JwtProvider;
using PetFamily.Auth.Infrastructure.Services.PasswordHasher;
using PetFamily.SharedInfrastructure.Shared.Constants;
using static PetFamily.Auth.Infrastructure.AuthInjector.JwtAuthenticationInjector;
using static PetFamily.Auth.Infrastructure.AuthInjector.PermissionsPolicesAuthorizationInjector;


namespace PetFamily.Auth.Infrastructure.AuthInjector;

public static class PetFamilyAuthInjector
{
    public static IServiceCollection InjectPetFamilyAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.GetConnectionString(ConnectionStringName.POSTGRESQL);
        if (string.IsNullOrEmpty(postgresConnection))
            throw new ApplicationException("PostgreSQL connection string wasn't found!");

        services.InjectPetFamilyAuthApplication(configuration);

        services
            .AddScoped<IRefreshTokenWriteRepository, RefreshTokenWriteRepository>()

            .AddScoped<IUserReadRepository, UserReadRepository>()
            .AddScoped<IUserWriteRepository, UserWriteRepository>()

            .AddScoped<IRoleReadRepository, RoleReadRepository>()
            .AddScoped<IRoleWriteRepository, RoleWriteRepository>()

            .AddScoped<IPermissionReadRepository, PermissionReadRepository>()
            .AddScoped<IPermissionWriteRepository, PermissionWriteRepository>()

            .AddScoped<IJwtProvider, JwtProvider>()
            .AddScoped<IPasswordHasher, PasswordHasher>()
            .AddScoped<IEmailService, EmailService>()
            .AddScoped<IAuthUnitOfWork, AuthUnitOfWork>()

            .AddScoped(_ => new AuthWriteDbContext(postgresConnection))
            .Configure<JwtOptions>(configuration.GetSection("JwtOptions"))

            .AddJwtAuthentication(configuration)

            .AddPermissionPoliciesAuthorization()

            .AddSingleton<IAuthorizationHandler, AuthorizationHandlerByPermissions>()

            .AddHttpContextAccessor();

        services.AddHostedService<AuthSoftDeletableCleanupService>();

        AuthDapperConverters.Register();

        return services;
    }
}
