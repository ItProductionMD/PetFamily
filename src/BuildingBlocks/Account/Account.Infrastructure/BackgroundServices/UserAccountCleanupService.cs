using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Account.Infrastructure.Contexts;
using PetFamily.SharedInfrastructure.SoftDeletableCleaner;

namespace Account.Infrastructure.BackgroundServices;

public class UserAccountCleanupService : SoftDeletableCleanerService
{
    public UserAccountCleanupService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<SoftDeletableCleanerService> logger,
        IConfiguration configuration) : base(serviceScopeFactory, logger, configuration)
    {
        ServiceName = "UserAccountCleanupService";
    }

    override public async Task RunCleanupAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UserWriteDbContext>();

            _logger.LogInformation("{ServiceName} starting cleanup task...", ServiceName);

            int deletedUsersCount = await dbContext.Users
                .Where(u =>
                    u.IsDeleted == true &&
                    u.DeletedAt <= DateTime.UtcNow.AddDays(-DeleteAfterDays))
                .ExecuteDeleteAsync(cancellationToken);


            _logger.LogInformation("{ServiceName} cleanup task completed, {deletedUsersCount} users deleted"
                , ServiceName, deletedUsersCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ServiceName} error occurred during cleanup task.", ServiceName);
        }
    }

}
