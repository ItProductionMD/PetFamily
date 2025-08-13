using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Auth.Application;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Infrastructure.BackgroundServices;
using PetFamily.Auth.Infrastructure.Contexts;
using PetFamily.Auth.Infrastructure.Dapper;
using PetFamily.Auth.Infrastructure.Repository;
using PetFamily.Auth.Infrastructure.Services.EmailService;
using PetFamily.Auth.Infrastructure.Services.JwtProvider;
using PetFamily.Auth.Infrastructure.Services.PasswordHasher;
using PetFamily.Auth.Public.Contracts;
using PetFamily.SharedApplication.Extensions;
using PetFamily.SharedInfrastructure.Shared.Constants;


namespace PetFamily.Auth.Infrastructure.AuthInjector;

public static class PetFamilyAuthInjector
{
    public static IServiceCollection InjectAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.TryGetConnectionString(ConnectionStringName.POSTGRESQL);

        services.InjectPetFamilyAuthApplication(configuration);

        services
            .AddScoped<IRefreshTokenWriteRepository, RefreshTokenWriteRepository>()

            .AddScoped<IUserReadRepository, UserReadRepository>()
            .AddScoped<IUserWriteRepository, UserWriteRepository>()
            .AddScoped<IUserContract, UserReadRepository>()

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

            .AddJwtAuthentication(configuration);

        services.AddHostedService<AuthSoftDeletableCleanupService>();

        AuthDapperConverters.Register();

        return services;
    }
}
