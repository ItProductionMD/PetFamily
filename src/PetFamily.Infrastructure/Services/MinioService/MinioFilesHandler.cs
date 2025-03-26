using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using Polly;
using Polly.Retry;


namespace PetFamily.Infrastructure.Services.MinioService;

public class MinioFilesHandler
{
    private readonly SemaphoreSlim _semaphore;
    private readonly CancellationToken _cancelToken;
    private readonly ILogger<MinioFileRepository> _logger;

    public MinioFilesHandler(
        ILogger<MinioFileRepository> logger,
        int maxParallelTasks,
        CancellationToken cancellationToken)
    {
        _logger = logger;
        _semaphore = new SemaphoreSlim(maxParallelTasks);
        _cancelToken = cancellationToken;
    }

    public async Task<Result<List<string>>> ProcessFilesAsync(
        List<AppFileDto> files,
        Func<AppFileDto, CancellationToken, Task> fileAction)
    {
        var handledFiles = new ConcurrentBag<string>();
        var errors = new ConcurrentBag<Error>();
        var tasks = new List<Task>();

        foreach (var file in files)
        {
            var task = ProcessFileAsync(file, fileAction, handledFiles, errors);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        if (errors.IsEmpty == false)
        {
            var errorsMessage = errors.Select(e=> e.Message).ToList();

            var sb = new StringBuilder();
            foreach (var error in errors)
            {
                sb.AppendLine(error.Message);
                sb.AppendLine(";");
            }
            _logger.LogError("ProcessFiles  error:{errors}",sb);

            return Result
                .Fail(Error.InternalServerError("Some files were not handled!"))
                .WithData(handledFiles.ToList());
        }

        return Result.Ok(handledFiles.ToList());
    }

    private async Task ProcessFileAsync(
        AppFileDto file,
        Func<AppFileDto, CancellationToken, Task> fileAction,
        ConcurrentBag<string> handledFiles,
        ConcurrentBag<Error> errors)
    {
        await _semaphore.WaitAsync();
        try
        {
            await fileAction(file, _cancelToken);
            handledFiles.Add(file.Name);
            return;
        }
        catch (OperationCanceledException)
        {
            errors.Add(Error.Cancelled($"Handle file:{file.Name} - task was cancelled!"));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(
                "Error while Proccess File with MinioFilesHandler!FileName:{Name};Exception:{ex}",
                file.Name, ex.Message);

            errors.Add(Error.InternalServerError($"ProcessFile: {file.Name} unexpected error!"));
        }
        finally
        {
            _semaphore.Release();
        }
    }
}