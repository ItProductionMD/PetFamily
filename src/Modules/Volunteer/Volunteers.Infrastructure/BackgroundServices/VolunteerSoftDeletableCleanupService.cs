using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.SharedInfrastructure.SoftDeletableCleaner;
using Volunteers.Infrastructure.Contexts;

namespace Volunteers.Infrastructure.BackgroundServices;

public class VolunteerSoftDeletableCleanupService : SoftDeletableCleanerService
{
    public VolunteerSoftDeletableCleanupService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<VolunteerSoftDeletableCleanupService> logger,
        IConfiguration configuration) : base(serviceScopeFactory, logger, configuration)
    {
        ServiceName = "VolunteerDeletableCleanupService";
    }

    override public async Task RunCleanupAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<VolunteerWriteDbContext>();

            _logger.LogInformation("{ServiceName} starting cleanup task...", ServiceName);

            int deletedVolunteersCount = await dbContext.Volunteers
                .Where(v =>
                    v.IsDeleted == true &&
                    v.DeletedAt <= DateTime.UtcNow.AddDays(-DeleteAfterDays))
                .ExecuteDeleteAsync(cancellationToken);

            int deletedPetsCount = await dbContext.Volunteers
                .SelectMany(v => v.Pets)
                .Where(p =>
                    p.IsDeleted == true &&
                    p.DeletedAt <= DateTime.UtcNow.AddDays(-DeleteAfterDays))
                .ExecuteDeleteAsync(cancellationToken);


            _logger.LogInformation("{ServiceName} cleanup task completed, {deletedVolunteersCount} volunteers deleted," +
                " and {deletedPetsCount} pets deleted"
                , ServiceName, deletedVolunteersCount, deletedPetsCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ServiceName} error occurred during cleanup task.", ServiceName);
        }
    }
}
