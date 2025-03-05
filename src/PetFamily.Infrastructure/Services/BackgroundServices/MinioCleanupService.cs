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
using PetFamily.Application.FilesManagment;
using PetFamily.Infrastructure.Services.MinioService;
using Minio.DataModel.Args;
using PetFamily.Application.FilesManagment.Dtos;

namespace PetFamily.Infrastructure.Services.BackgroundServices;

public class MinioCleanupService(
    IOptions<MinioOptions> minioOptions,
    IOptions<FileFolders> fileFoders,
    IMinioClient client,
    ILogger<MinioCleanupService> logger) : BackgroundService
{
    private readonly ILogger<MinioCleanupService> _logger = logger;
    private readonly IMinioClient _minioClient = client;
    private readonly MinioOptions _mionioOptions = minioOptions.Value;
    private readonly FileFolders _fileFolders = fileFoders.Value;
    private string deletionBucket => _fileFolders.PendingDeletion;
    private readonly TimeSpan _fileLifetime = TimeSpan.FromMinutes(10);
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(24*60); 

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Minio Background Cleanup Service started!");
                await CleanupOldFilesAsync();
                _logger.LogInformation("Minio Background Cleanup Service ended!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Minio Background Cleanup Service error!");
            }
            await Task.Delay(_checkInterval, stoppingToken); 
        }
    }

    private async Task CleanupOldFilesAsync()
    {     
        bool bucketExists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(deletionBucket));
        if (bucketExists == false)      
            return;

        var now = DateTime.UtcNow;
        List<string> filesToDelete = new();

        var listObjectsArgs = new ListObjectsArgs()
            .WithBucket(deletionBucket)
            .WithRecursive(true);
        IAsyncEnumerable<Item> items = _minioClient.ListObjectsEnumAsync(listObjectsArgs);

        await foreach (var item in items)
        {
            var lastModifiedUtc = item.LastModifiedDateTime!.Value.ToUniversalTime();
            if (now - lastModifiedUtc > _fileLifetime)
            {
                filesToDelete.Add(item.Key);
            }
        }

        foreach (var file in filesToDelete)
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(deletionBucket)
                .WithObject(file);

            await _minioClient.RemoveObjectAsync(removeObjectArgs);

            _logger.LogInformation("Deleted File {file}",file);
        }

        _logger.LogInformation("Minio Background Cleanup service deleted: {Count} files",filesToDelete.Count);
    }
}

