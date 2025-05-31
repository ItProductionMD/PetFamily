using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volunteers.Infrastructure.Contexts;

namespace Volunteers.Infrastructure.BackgroundServices;

public class DbCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<DbCleanupService> _logger;
    private readonly IConfiguration _configuration;
    private int _deleteAfterDays;
    private int _timeDelayInHours;

    public DbCleanupService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DbCleanupService> logger,
        IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _configuration = configuration;
        var cleanupOptions = _configuration.GetSection("DbCleanupService")
            ?? throw new ApplicationException("DbCleanupService configuration wasn't found!");

        _deleteAfterDays = cleanupOptions.GetValue<int>("DeleteAfterDays");

        _timeDelayInHours = cleanupOptions.GetValue<int>("TimeDelayInHours");
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {

        _logger.LogInformation("CleanupService started. DeleteAfterDays: {_deleteAfterDays}," +
            " TimeDelayInHours: {_timeDelayInHours}", _deleteAfterDays, _timeDelayInHours);

        await RunCleanupAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Waiting {_timeDelayInHours} hours before next cleanup cycle.",
                    _timeDelayInHours);
                await Task.Delay(TimeSpan.FromHours(_timeDelayInHours), cancellationToken);

                await RunCleanupAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while running the cleanup task.");
            }
        }
    }

    private async Task RunCleanupAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<VolunteerWriteDbContext>();

            _logger.LogInformation("Starting cleanup task...");

            int deletedVolunteersCount = await dbContext.Volunteers.Where(v =>
                EF.Property<bool>(v, "_isDeleted") == true
                && EF.Property<DateTime?>(v, "_deletedDateTime")!.Value
                <= DateTime.UtcNow.AddDays(-_deleteAfterDays))
            .ExecuteDeleteAsync(cancellationToken);

            int deletedPetsCount = await dbContext.Volunteers
                .SelectMany(v => v.Pets)
                .Where(p =>
                    EF.Property<bool>(p, "_isDeleted") == true &&
                    EF.Property<DateTime?>(p, "_deletedDateTime")!.Value
                    <= DateTime.UtcNow.AddDays(-_deleteAfterDays))
                .ExecuteDeleteAsync(cancellationToken);


            _logger.LogInformation("Cleanup task completed, {deletedVolunteersCount} volunteers deleted," +
                " and {deletedPetsCount} pets deleted",
                deletedVolunteersCount, deletedPetsCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during cleanup task.");
        }
    }
}
