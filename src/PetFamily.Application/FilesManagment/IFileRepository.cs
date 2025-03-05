using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Application.FilesManagment.Dtos;
using PetFamily.Domain.Results;

namespace PetFamily.Application.FilesManagment;

public interface IFileRepository
{
    public Task<Result<Uri>> GetFileUrlAsync(AppFile file,CancellationToken cancelToken);

    public Task UploadFileAsync(AppFile file,CancellationToken cancelToken);

    public Task<Result<List<string>>> UploadFileListAsync(
        List<AppFile> file,
        CancellationToken cancelToken);
    public Task DeleteFileAsync(AppFile file,CancellationToken cancelToken);

    public Task<Result<List<string>>> DeleteFileListAsync(
       List<AppFile> file,
       CancellationToken cancelToken);

    public Task SoftDeleteFileAsync(AppFile file, CancellationToken cancelToken);

    public Task<Result<List<string>>> SoftDeleteFileListAsync(
        List<AppFile> files,
        CancellationToken cancelToken);

    public Task RestoreFileAsync(AppFile file, CancellationToken cancelToken);
    public Task RestoreFileWithRetryAsync(AppFile file, CancellationToken cancelToken = default);
    public Task<Result<List<string>>> RestoreFileListAsync(
        List<AppFile> files,
        CancellationToken cancelToken);

    public Task<UnitResult> RollBackFilesAsync(
        List<AppFile> filesToRestore,
        List<AppFile> filesToDelete,
        CancellationToken cancelToken = default);
}

