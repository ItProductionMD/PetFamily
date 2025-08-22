using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Extensions;

namespace PetFamily.SharedInfrastructure.SoftDeletableCleaner;

public abstract class SoftDeletableCleanerService : BackgroundService
{
    public string ServiceName { get; set; } = "DefaultServiceName";
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public IServiceScopeFactory ServiceScopeFactory => _serviceScopeFactory;
    private readonly IConfiguration _configuration;
    public IConfiguration Configuration => _configuration;
    private readonly int _deleteAfterDays;
    public int DeleteAfterDays => _deleteAfterDays;
    private readonly int timeDelayInHours;
    public int TimeDelayInHours => timeDelayInHours;

    public readonly ILogger<SoftDeletableCleanerService> _logger;

    public SoftDeletableCleanerService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<SoftDeletableCleanerService> logger,
        IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _configuration = configuration;

        _configuration.CheckSectionsExistence(["DbCleanUpService"]);

        var cleanupOptions = _configuration.GetSection("DbCleanupService");

        _deleteAfterDays = cleanupOptions.GetValue<int>("DeleteAfterDays");

        timeDelayInHours = cleanupOptions.GetValue<int>("TimeDelayInHours");
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(3000000);

        _logger.LogInformation("{serviceName} started. DeleteAfterDays: {_deleteAfterDays}," +
            " TimeDelayInHours: {_timeDelayInHours}", ServiceName, _deleteAfterDays, timeDelayInHours);

        await RunCleanupAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("{ServiceName} Waiting {_timeDelayInHours} hours before next cleanup cycle.",
                    ServiceName, timeDelayInHours);
                await Task.Delay(TimeSpan.FromHours(timeDelayInHours), cancellationToken);

                await RunCleanupAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while running the {serviceName} cleanup task."
                    , ServiceName);
            }
        }
    }

    public virtual async Task RunCleanupAsync(CancellationToken cancellationToken)
    {
    }
}
