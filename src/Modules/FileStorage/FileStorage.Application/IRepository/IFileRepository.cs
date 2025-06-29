using FileStorage.Public.Dtos;
using PetFamily.SharedKernel.Results;

namespace FileStorage.Application.IRepository;

public interface IFileRepository
{
    public Task<Result<Uri>> GetFileUrlAsync(FileDto file, CancellationToken cancelToken);

    public Task UploadFileAsync(UploadFileDto file, CancellationToken cancelToken);

    public Task<Result<List<FileUploadResponse>>> UploadFilesAsync(
        List<UploadFileDto> fileDto,
        CancellationToken cancelToken);

    public Task DeleteFileAsync(FileDto file, CancellationToken cancelToken);

    public Task<Result<List<FileDeleteResponse>>> DeleteFileListAsync(
       List<FileDto> file,
       CancellationToken cancelToken);

    public Task SoftDeleteFileAsync(FileDto file, CancellationToken cancelToken);

    public Task<Result<List<string>>> SoftDeleteFilesAsync(
        List<FileDto> files,
        CancellationToken cancelToken);

    public Task RestoreFileAsync(FileDto file, CancellationToken cancelToken);

    public Task RestoreFileWithRetryAsync(FileDto file, CancellationToken cancelToken = default);

    public Task<Result<List<string>>> RestoreFilesAsync(
        List<FileDto> files,
        CancellationToken cancelToken = default);

    public Task<UnitResult> RollBackFilesAsync(
        List<FileDto> filesToRestore,
        List<FileDto> filesToDelete,
        CancellationToken cancelToken = default);
}

