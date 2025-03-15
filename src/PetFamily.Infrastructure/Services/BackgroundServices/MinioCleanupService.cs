using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using System.Collections.Generic;
using Minio.ApiEndpoints;
using Microsoft.Extensions.Options;
using PetFamily.Infrastructure.Services.MinioService;
using Minio.DataModel.Args;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Dtos;

namespace PetFamily.Infrastructure.Services.BackgroundServices;

public class MinioCleanupService(
    ILogger<MinioCleanupService> logger,
    FilesProcessingQueue filesProcessingQueue,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly ILogger<MinioCleanupService> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly FilesProcessingQueue _filesQueue = filesProcessingQueue;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var fileList in _filesQueue.DeleteChannel.Reader.ReadAllAsync(stoppingToken))
        {
            _logger.LogInformation("Minio Background Cleanup Service started!");

            await CleanupOldFilesAsync(fileList,stoppingToken);

            _logger.LogInformation("Minio Background Cleanup Service ended!");
        }

    }

    private async Task CleanupOldFilesAsync(List<AppFile> fileList,CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning("Minio Background Cleanup Service is stopping.");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var fileService = scope.ServiceProvider.GetRequiredService<IFileRepository>();

        var result = await fileService.SoftDeleteFileListAsync(fileList, stoppingToken);
        if (result.IsFailure)
        {
            if (result.Data != null && result.Data.Count != 0)
            {
                var undeletedFiles = fileList
                    .Select(f => f.Name)
                    .Except(result.Data).ToList();

                string names = string.Join(";", result.Data);

                _logger.LogCritical("UndeletedFiles:{fileNames}", names);
            }
            _logger.LogCritical("MinioBackgroundCleanup service error!Errors:{error}"
                , result.ToErrorMessages());
        }
        else
        {
            string names = string.Join(";", fileList.Select(f=>f.Name));
            _logger.LogInformation("Minio Background Cleanup Service removed files:{names}", names);
        }
    }
}

