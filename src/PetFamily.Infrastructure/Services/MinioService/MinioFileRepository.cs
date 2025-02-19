using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using Polly;
using Polly.Retry;
using System.Collections.Concurrent;
using System.Security.AccessControl;
using System.Threading;

namespace PetFamily.Infrastructure.Services.MinioService;

public class MinioFileRepository : IFileRepository
{
    private readonly IMinioClient _client;
    private readonly ILogger<MinioFileRepository> _logger;
    public int RETRY_COUNT = 3;
    public int MAX_PARALLEL_TASKS = 3;//for semaphoreSlim
    public MinioFileRepository(
        IOptions<MinioOptions> minioOtions,
        IMinioClient client,
        ILogger<MinioFileRepository> logger,
        IConfiguration configuration)
    {
        _client = client;
        _logger = logger;
    }
    //-------------------------------------------Get File-----------------------------------------//
    public async Task<Result<Uri>> GetFileUrlAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken)
    {
        var existArgs = new BucketExistsArgs().WithBucket(bucketName);
        var bucketExist = await _client.BucketExistsAsync(existArgs, cancellationToken);
        if (bucketExist)
        {
            try
            {
                await _client.StatObjectAsync(new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    , cancellationToken);

                var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithExpiry(60 * 60 * 24)); // 24 часа

                var uri = new Uri(url);

                return uri == null ? Result.Fail(Error.NotFound("File in minio")) : Result.Ok(uri);
            }
            catch (Exception ex)
            {
                _logger.LogError("File with name:{objectName} not found!Exception:{ex}",
                    objectName, ex);

                return Result.Fail(Error.NotFound("File(minio) not found!"));
            }
        }
        _logger.LogError("Bucket with name:{bucketName} not found!", bucketName);
        return Result.Fail(Error.NotFound("Bucket(minio) not found!"));
    }
   
    //------------------------------------------Upload File---------------------------------------//
    public async Task UploadFileAsync(
        string bucketName,
        UploadFileCommand fileCommand,
        CancellationToken cancellationToken)
    {
        var retryPolicy = Policy
            .Handle<MinioException>()
            .WaitAndRetryAsync(RETRY_COUNT, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (Action<Exception, TimeSpan, int, Context>)((exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry {RetryCount}: Failed to upload file '{FileName}' to bucket" +
                        " '{BucketName}'. Waiting {TimeSpan} before retrying.",
                        retryCount,
                        (object)fileCommand.StoredName,
                        bucketName,
                        timeSpan);
                }));
        await retryPolicy.ExecuteAsync((Func<Task>)(async () =>
        {
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject((string)fileCommand.StoredName)
                .WithStreamData(fileCommand.Stream)
                .WithObjectSize(fileCommand.Size);

            if (!string.IsNullOrEmpty(fileCommand.MimeType))
                putObjectArgs = putObjectArgs.WithContentType(fileCommand.MimeType);

            await _client.PutObjectAsync(putObjectArgs, cancellationToken);
            _logger.LogInformation(
              "Uploaded file {ObjectName} to MinIO bucket {BucketName}", (object)fileCommand.StoredName, bucketName);
        }));
    }

    //------------------------------------------Upload FileList-----------------------------------//
    public async Task<Result<List<string>>> UploadFileListAsync(
        string bucketName,
        List<UploadFileCommand> fileCommands,
        CancellationToken cancellationToken
        )
    {
        var minioFilesHandler = new MinioFilesHandler<UploadFileCommand>(
            MAX_PARALLEL_TASKS,
            cancellationToken);

        return await minioFilesHandler.ProcessFilesAsync(bucketName, fileCommands, UploadFileAsync);

    }
    //--------------------------------------------Delete File-------------------------------------//
    public async Task DeleteFileAsync(
       string bucketName,
       DeleteFileCommand fileCommand,
       CancellationToken cancellationToken)
    {
        var retryPolicy = Policy
           .Handle<MinioException>()
           .WaitAndRetryAsync(RETRY_COUNT, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
               (exception, timeSpan, retryCount, context) =>
               {
                   _logger.LogWarning(
                       exception,
                       "Retry {RetryCount}: Failed to delete file '{Name}' to bucket" +
                       " '{bucketName}'. Waiting {timeSpan} before retrying.",
                       retryCount,
                       fileCommand.StoredName,
                       bucketName,
                       timeSpan);
               });

        await retryPolicy.ExecuteAsync(async () =>
        {
            var removeArgs = new RemoveObjectArgs().WithBucket(bucketName).WithObject(fileCommand.StoredName);

            await _client.RemoveObjectAsync(removeArgs, cancellationToken);

            _logger.LogInformation(
                "Deleted file {ObjectName} from MinIO bucket {BucketName}", fileCommand.StoredName, bucketName);
        });
    }

    //-------------------------------------------Delete FileList----------------------------------//
    public async Task<Result<List<string>>> DeleteFileListAsync(
    string bucketName,
    List<DeleteFileCommand> fileCommands,
    CancellationToken cancellationToken)
    {
        var minioFilesHandler = new MinioFilesHandler<DeleteFileCommand>(
            MAX_PARALLEL_TASKS, 
            cancellationToken);

        return await minioFilesHandler.ProcessFilesAsync(bucketName, fileCommands, DeleteFileAsync);
    }

}
