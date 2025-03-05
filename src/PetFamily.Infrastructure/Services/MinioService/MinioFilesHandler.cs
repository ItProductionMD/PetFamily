using System.Collections.Concurrent;
using System.Collections.Generic;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Application.FilesManagment.Dtos;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using Polly;
using Polly.Retry;


namespace PetFamily.Infrastructure.Services.MinioService;

public class MinioFilesHandler
{
    private readonly SemaphoreSlim _semaphore;
    private readonly CancellationToken _cancelToken;

    public MinioFilesHandler(
        int maxParallelTasks,
        CancellationToken cancellationToken)
    {
        _semaphore = new SemaphoreSlim(maxParallelTasks);
        _cancelToken = cancellationToken;
    }

    public async Task<Result<List<string>>> ProcessFilesAsync(
        List<AppFile> files,
        Func<AppFile, CancellationToken, Task> fileAction)
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

        if (errors.Count > 0)
            return Result
                .Fail(errors.ToList())
                .WithData(handledFiles.ToList());

        return Result.Ok(handledFiles.ToList());
    }

    private async Task ProcessFileAsync(
        AppFile file,
        Func<AppFile, CancellationToken, Task> fileAction,
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
            errors.Add(Error.Exception(ex));
        }
        finally
        {
            _semaphore.Release();
        }
    }
}