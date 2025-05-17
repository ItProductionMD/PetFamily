using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Domain.Results;

namespace PetFamily.Application.IRepositories;

public interface IFileRepository
{
    public Task<Result<Uri>> GetFileUrlAsync(AppFileDto file, CancellationToken cancelToken);

    public Task UploadFileAsync(AppFileDto file, CancellationToken cancelToken);

    public Task<Result<List<FileUploadResponse>>> UploadFileListAsync(
        List<AppFileDto> file,
        CancellationToken cancelToken);

    public Task DeleteFileAsync(AppFileDto file, CancellationToken cancelToken);

    public Task<Result<List<FileDeleteResponse>>> DeleteFileListAsync(
       List<AppFileDto> file,
       CancellationToken cancelToken);

    public Task SoftDeleteFileAsync(AppFileDto file, CancellationToken cancelToken);

    public Task<Result<List<string>>> SoftDeleteFileListAsync(
        List<AppFileDto> files,
        CancellationToken cancelToken);

    public Task RestoreFileAsync(AppFileDto file, CancellationToken cancelToken);

    public Task RestoreFileWithRetryAsync(AppFileDto file, CancellationToken cancelToken = default);

    public Task<Result<List<string>>> RestoreFileListAsync(
        List<AppFileDto> files,
        CancellationToken cancelToken = default);

    public Task<UnitResult> RollBackFilesAsync(
        List<AppFileDto> filesToRestore,
        List<AppFileDto> filesToDelete,
        CancellationToken cancelToken = default);
}

