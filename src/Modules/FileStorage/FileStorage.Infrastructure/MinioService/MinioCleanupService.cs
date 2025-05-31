using FileStorage.Application.IRepository;
using FileStorage.Infrastructure.MessageQueue;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileStorage.Infrastructure.MinioService;

public class MinioCleanupService(
    ILogger<MinioCleanupService> logger,
    IFileMessageQueue filesProcessingQueue,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly ILogger<MinioCleanupService> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IFileMessageQueue _filesQueue = filesProcessingQueue;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var fileList in _filesQueue.DeleteReader.ReadAllAsync(stoppingToken))
        {
            _logger.LogInformation("Minio Background Cleanup Service started!");

            await CleanupOldFilesAsync(fileList, stoppingToken);

            _logger.LogInformation("Minio Background Cleanup Service ended!");
        }

    }

    private async Task CleanupOldFilesAsync(List<FileDto> fileList, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning("Minio Background Cleanup Service is stopping.");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var fileService = scope.ServiceProvider.GetRequiredService<IFileRepository>();

        var result = await fileService.SoftDeleteFilesAsync(fileList, stoppingToken);
        if (result.IsFailure)
        {
            _logger.LogCritical("MinioBackgroundCleanup service error!Errors:{error}", result.Error.Message);

            if (result.Data != null && result.Data.Count != 0)
            {
                var undeletedFiles = fileList
                    .Select(f => f.Name)
                    .Except(result.Data).ToList();

                string names = string.Join(";", undeletedFiles);

                _logger.LogCritical("UndeletedFiles:{fileNames}", names);
            }
            else
            {
                var undeletedFiles = fileList
                    .Select(f => f.Name)
                    .ToList();

                string names = string.Join(";", undeletedFiles);

                _logger.LogCritical("UndeletedFiles:{fileNames}", names);
            }
        }
        else
        {
            string names = string.Join(";", fileList.Select(f => f.Name));
            _logger.LogInformation("Minio Background Cleanup Service removed files:{names}", names);
        }
    }
}

