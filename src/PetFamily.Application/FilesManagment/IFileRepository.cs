using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Domain.Results;

namespace PetFamily.Application.FilesManagment;

public interface IFileRepository
{
    public Task<Result<Uri>> GetFileUrlAsync(
        string folder,
        string objectName,
        CancellationToken cancellationToken);

    public Task UploadFileAsync(
       string folder,
       UploadFileCommand fileCommand,
       CancellationToken cancellationToken);

    public Task<Result<List<string>>> UploadFileListAsync(
         string folder,
         List<UploadFileCommand> fileCommands,
         CancellationToken cancellationToken);
    public Task DeleteFileAsync(
       string folder,
       DeleteFileCommand fileCommand,
       CancellationToken cancellationToken);

    public Task<Result<List<string>>> DeleteFileListAsync(
       string folder,
       List<DeleteFileCommand> fileCommands,
       CancellationToken cancellationToken);
}

