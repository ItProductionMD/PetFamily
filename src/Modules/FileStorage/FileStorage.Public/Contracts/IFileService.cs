using FileStorage.Public.Dtos;
using PetFamily.SharedKernel.Results;

namespace FileStorage.Public.Contracts;

public interface IFileService
{
    Task<Result<List<FileUploadResponse>>> UploadFilesAsync(
        List<FileDto> fileDtos,
        CancellationToken ct = default);

    Task<Result<List<FileDeleteResponse>>> DeleFilesAsync(
        List<FileDto> fileDtos,
        CancellationToken ct = default);

    Task<Result<Uri>> GetFileUrlAsync(
        FileDto fileDto,
        CancellationToken ct = default);

    Task DeleteFilesUsingMessageQueue
        (List<FileDto> fileDtos,
        CancellationToken ct = default);

    Task<Result<List<string>>> RestoreFilesAsync
        (List<FileDto> fileDtos,
        CancellationToken ct = default);
}
