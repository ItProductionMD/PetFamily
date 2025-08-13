using FileStorage.Application.IRepository;
using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using PetFamily.SharedKernel.Results;

namespace FileStorage.Application;

public class FileService(
    IFileRepository repository,
    IFileScheduler scheduler) : IFileService
{
    private readonly IFileRepository _repository = repository;
    private readonly IFileScheduler _scheduler = scheduler;

    public async Task<Result<List<FileUploadResponse>>> UploadFilesAsync(
        List<UploadFileDto> fileDtos,
        CancellationToken ct = default) =>
        await _repository.UploadFilesAsync(fileDtos, ct);

    public async Task<Result<List<FileDeleteResponse>>> DeleFilesAsync(
        List<FileDto> fileDtos,
        CancellationToken ct = default) =>
        await _repository.DeleteFileListAsync(fileDtos, ct);

    public async Task<Result<Uri>> GetFileUrlAsync(
        FileDto fileDto,
        CancellationToken ct = default) =>
        await _repository.GetFileUrlAsync(fileDto, ct);

    public async Task DeleteFilesUsingMessageQueue
        (List<FileDto> fileDtos,
        CancellationToken ct = default) =>
        await _scheduler.AddToDeletionQueue(fileDtos, ct);

    public async Task<Result<List<string>>> RestoreFilesAsync(
        List<FileDto> fileDtos,
        CancellationToken ct = default) =>
        await _repository.RestoreFilesAsync(fileDtos, ct);
}
