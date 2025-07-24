using FileStorage.Application;
using FileStorage.Public.Dtos;
using FileStorage.Infrastructure.MessageQueue;

namespace FileStorage.Infrastructure.Scheduler;

public class FileScheduler(IFileMessageQueue fileMessageQueue ) : IFileScheduler
{
    private readonly IFileMessageQueue _filesMessageQueue = fileMessageQueue;
    public async Task AddToDeletionQueue(List<FileDto> filesToDelete, CancellationToken ct = default)
    {
        await _filesMessageQueue.PublicToDeletionMessage(filesToDelete, ct);
    }
}
