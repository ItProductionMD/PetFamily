using System.Collections.Concurrent;
using System.Collections.Generic;
using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using Polly;
using Polly.Retry;


namespace PetFamily.Infrastructure.Services.MinioService;

public class MinioFilesHandler<T> where T : IFileCommand
{
    private readonly SemaphoreSlim _semaphore;
    private readonly CancellationToken _cancellationToken;

    public MinioFilesHandler(
        int maxParallelTasks,
        CancellationToken cancellationToken)
    {
        _semaphore = new SemaphoreSlim(maxParallelTasks);
        _cancellationToken = cancellationToken;
    }

    public async Task<Result<List<string>>> ProcessFilesAsync(
        string bucketName,
        List<T> fileCommands,
        Func<string, T, CancellationToken, Task> fileAction)
    {
        var handledFiles = new ConcurrentBag<string>();
        var errors = new ConcurrentBag<Error>();
        var tasks = new List<Task>();

        foreach (var file in fileCommands)
        {
            var task = ProcessFileAsync(file, bucketName, fileAction, handledFiles, errors);
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
        T fileCommand,
        string bucketName,
        Func<string,T, CancellationToken, Task> fileAction,
        ConcurrentBag<string> handledFiles,
        ConcurrentBag<Error> errors)
    {
        await _semaphore.WaitAsync();
        try
        {
            await fileAction(bucketName, fileCommand, _cancellationToken);
            handledFiles.Add(fileCommand.StoredName);
            return;
        }
        catch (OperationCanceledException)
        {
            errors.Add(Error.Cancelled($"Handle file:{fileCommand.StoredName} - task was cancelled!"));

            return;
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
