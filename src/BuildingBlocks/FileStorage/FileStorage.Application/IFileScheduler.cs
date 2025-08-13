using FileStorage.Public.Dtos;

namespace FileStorage.Application;

public interface IFileScheduler
{
    Task AddToDeletionQueue(
        List<FileDto> filesToDelete,
        CancellationToken ct = default);
}
