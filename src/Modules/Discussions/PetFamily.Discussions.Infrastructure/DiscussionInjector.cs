using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Discussions.Infrastructure.Contracts;
using PetFamily.Discussions.Public.Contracts;

namespace PetFamily.Discussions.Infrastructure;

public static class DiscussionInjector
{
    public static IServiceCollection InjectDiscussionModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddScoped<IDiscussionMessageSender, DiscussionMessageSender>()
            .AddScoped<IMessageRemover, MessageRemover>()
            .AddScoped<IDiscussionRemover, DiscussionRemover>()
            .AddScoped<IDiscussionCreator, DisscussionCreator>();

        return services;
    }
}
