using PetFamily.Domain.Shared.DomainResult;

namespace PetFamily.Application;

public interface IAppiFileService
{
    public Task<Result<Guid>> UploadFileAsync(
        string bucketName,
        string objectName,
        Stream stream,
        CancellationToken cancellationToken);
    public Task<Result<Guid>> RemoveFileAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken);
    public Task<Result<Uri>> GetFileUrl(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken);
}

