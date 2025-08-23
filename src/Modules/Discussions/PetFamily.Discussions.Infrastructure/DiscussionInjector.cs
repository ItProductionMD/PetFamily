using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Discussions.Application;
using PetFamily.Discussions.Application.IRepositories;
using PetFamily.Discussions.Infrastructure.Contexts;
using PetFamily.Discussions.Infrastructure.Contracts;
using PetFamily.Discussions.Infrastructure.Dapper;
using PetFamily.Discussions.Infrastructure.Repositories;
using PetFamily.Discussions.Public.Contracts;
using PetFamily.SharedApplication.DependencyInjection;
using PetFamily.SharedInfrastructure.Shared.Constants;

namespace PetFamily.Discussions.Infrastructure;

public static class DiscussionInjector
{
    public static IServiceCollection AddDiscussionModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.TryGetConnectionString(ConnectionStringName.POSTGRESQL);

        services.InjectDiscussionApplication(configuration);

        services
            .AddScoped<IDiscussionWriteRepository, DiscussionWriteRepository>()
            .AddScoped<IDiscussionReadRepository, PetFamily.Discussions.Infrastructure.Repositories.DiscussionReadRepository>()
            .AddScoped<IDiscussionMessageSender, DiscussionMessageSender>()
            .AddScoped<IMessageRemover, MessageRemover>()
            .AddScoped<IDiscussionRemover, DiscussionRemover>()
            .AddScoped<IDiscussionCreator, DisscussionCreator>()
            .AddScoped<DiscussionWriteDbContext>(_ => new DiscussionWriteDbContext(postgresConnection));

        DiscussionMapperConvertors.Register();

        return services;
    }
}
