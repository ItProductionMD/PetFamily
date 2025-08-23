using Account.Application;
using Account.Application.DefaultSeeder;
using Account.Application.IRepositories;
using Account.Application.IServices;
using Account.Application.IServices.Email;
using Account.Application.Options;
using Account.Infrastructure.BackgroundServices;
using Account.Infrastructure.Contexts;
using Account.Infrastructure.Dapper;
using Account.Infrastructure.Repository;
using Account.Infrastructure.Services.EmailService;
using Account.Infrastructure.Services.PasswordHasher;
using Account.Public.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedApplication.DependencyInjection;
using PetFamily.SharedInfrastructure.Shared.Constants;
using static Account.Application.AccountCommandsAndQueriesInjector;


namespace Account.Infrastructure;

public static class AccountInfrastructureInjector
{
    public static IServiceCollection AddAccount(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.TryGetConnectionString(ConnectionStringName.POSTGRESQL);

        services.Configure<AdminIdentity>(configuration.GetSection(AdminIdentity.SECTION_NAME));
        services.Configure<RefreshTokenCookie>(configuration.GetSection(RefreshTokenCookie.SECTION_NAME));


        services
            .InjectCommandsAndQueries()
            
            .AddCustomOptions<AdminIdentity>(configuration)
            //.AddCustomOptions<RefreshTokenCookie>(configuration)

            .AddScoped<IUserReadRepository, UserReadRepository>()
            .AddScoped<IUserWriteRepository, UserWriteRepository>()
            .AddScoped<IUserContract, UserReadRepository>()
            .AddScoped<IUserAccountUnitOfWork, UserAccountUnitOfWork>()

            .AddScoped<IEmailService, EmailService>()
            .AddScoped<IEmailConfirmationTokenProvider, EmailConfirmationTokenProvider>()

            .AddScoped<IPasswordHasher, PasswordHasher>()

            .AddScoped(_ => new UserWriteDbContext(postgresConnection))

            .AddTransient<AdminSeeder>()

            .AddHostedService<UserAccountCleanupService>();

        AuthDapperConverters.Register();

        return services;
    }
}
