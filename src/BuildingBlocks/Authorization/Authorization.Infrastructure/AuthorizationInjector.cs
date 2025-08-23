using Authorization.Application;
using Authorization.Application.DefaultSeeder;
using Authorization.Application.IAuthorizationToken;
using Authorization.Application.IRepositories.IAuthorizationRepo;
using Authorization.Application.IRepositories.IRefreshTokenSessionRepo;
using Authorization.Infrastructure.AuthorizationToken;
using Authorization.Infrastructure.Contexts;
using Authorization.Infrastructure.Contracts;
using Authorization.Infrastructure.Dapper;
using Authorization.Infrastructure.Repositories.AuthorizationRepo;
using Authorization.Infrastructure.Repositories.RefreshTokenSessionRepo;
using Authorization.Public.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedApplication.DependencyInjection;
using PetFamily.SharedInfrastructure.Shared.Constants;

namespace Authorization.Infrastructure;

public static class AuthorizationInjector
{
    public static IServiceCollection AddAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.TryGetConnectionString(ConnectionStringName.POSTGRESQL);

        services
            .AddCommandsAndQueries<AuthorizationApplicationAssemblyReference>()

            .AddScoped<IAuthorizationTokenProvider, AuthorizationTokenProvider>()

            .AddScoped<IPermissionWriteRepo, PermissionWriteRepo>()
            .AddScoped<IPermissionReadRepo, PermissionReadRepo>()
            .AddScoped<IRoleReadRepo, RoleReadRepo>()
            .AddScoped<IRoleWriteRepo, RoleWriteRepo>()
            .AddScoped<IRefreshTokenWriteRepo, RefreshTokenWriteRepo>()

            .AddScoped<AuthorizationWriteDbContext>(_ => new AuthorizationWriteDbContext(postgresConnection))
            .AddScoped<JwtTokenWriteDbContext>(_ => new JwtTokenWriteDbContext(postgresConnection))

            .AddScoped<IAdminAuthorizationCreator, AdminAuthorizationCreator>()
            .AddScoped<IAuthorizationTokenContract, AuthorizationTokenContract>()
            .AddScoped<IRoleContract, RoleContract>()

            .AddTransient<RolesSeeder>();

        AuthDapperConverters.Register();

        return services;

    }
}
