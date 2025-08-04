using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetFamily.VolunteerRequests.Domain.Enums;
using PetFamily.VolunteerRequests.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.VolunteerRequests.Infrastructure.BGServices;

public class RejectedStatusProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<RejectedStatusProcessor> logger) : BackgroundService
{
    private const int DAYS_TO_RESTORE = 7;
    private const int BatchSize = 100;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<RejectedStatusProcessor> _logger = logger;
    private static readonly TimeSpan Delay = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan RejectedTimeout = TimeSpan.FromDays(DAYS_TO_RESTORE);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RejectedStatusProcessor started");
        await Task.Delay(3000000);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await TryRestoreRequest(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing rejected items");
            }
            try
            {
                await Task.Delay(Delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Delay cancelled, shutting down.");
                break;
            }
        }

        _logger.LogInformation("RejectedStatusProcessor stopped");
    }

    public async Task TryRestoreRequest(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VolunteerRequestDbContext>();

        var threshold = DateTime.UtcNow - RejectedTimeout;

        var volunteerRequests = await db.VolunteerRequests
            .Where(x => x.RequestStatus == RequestStatus.Rejected && x.RejectedAt <= threshold)
            .OrderBy(x => x.RejectedAt)
            .Take(BatchSize)
            .ToListAsync(stoppingToken);

        if (volunteerRequests.Count > 0)
        {
            foreach (var request in volunteerRequests)
            {
                var result = request.TryRestoreAfterRejectionExpiration(DAYS_TO_RESTORE);

                if (result.IsFailure)
                {
                    _logger.LogWarning("Failed to restore request {Id}: {Error}",
                        request.Id, result.Error);
                }
                else
                {
                    _logger.LogInformation("Request {Id} restored after rejection expiration",
                        request.Id);
                }
            }

            await db.SaveChangesAsync(stoppingToken);

            _logger.LogInformation("Updated {Count} rejected items", volunteerRequests.Count);
        }
        else
        {
            _logger.LogInformation("No expired rejected requests found at {Time}", DateTime.UtcNow);
        }
    }
}

