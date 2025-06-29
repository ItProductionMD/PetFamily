using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using FileStorage.Application.IRepository;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.ILM;
using Minio.Exceptions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Polly;
using LifecycleConfiguration = Minio.DataModel.ILM.LifecycleConfiguration;
using LifecycleRule = Minio.DataModel.ILM.LifecycleRule;


namespace FileStorage.Infrastructure.MinioService;


public class MinioFileRepository(
    IOptions<MinioOptions> minioOptions,
    IMinioClient client,
    IAmazonS3 amazonClient,
    ILogger<MinioFileRepository> logger) : IFileRepository 
{
    private readonly IMinioClient _client = client;
    private readonly IAmazonS3 _amazonClient = amazonClient;
    private readonly ILogger<MinioFileRepository> _logger = logger;
    private readonly MinioOptions _minioOptions = minioOptions.Value;

    //-------------------------------------------Get File-----------------------------------------//
    public async Task<Result<Uri>> GetFileUrlAsync(FileDto file, CancellationToken ct)
    {
        var existArgs = new BucketExistsArgs().WithBucket(file.Folder);
        var bucketExist = await _client.BucketExistsAsync(existArgs, ct);
        if (bucketExist == false)
        {
            _logger.LogError("Bucket with name:{bucketName} not found!", file.Name);
            return Result.Fail(Error.NotFound($"Bucket: {file.Folder} not found!"));
        }
        try
        {
            await _client.StatObjectAsync(new StatObjectArgs()
                .WithBucket(file.Folder)
                .WithObject(file.Name),
                 ct);

            var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                .WithBucket(file.Folder)
                .WithObject(file.Name)
                .WithExpiry(60 * 60 * 24)); // 24 часа

            var uri = new Uri(url);

            return Result.Ok(uri);
        }
        catch (Exception ex)
        {
            _logger.LogError("File with name:{objectName} not found!Exception:{ex}",
                file.Name, ex);

            return Result.Fail(Error.NotFound("File(minio) not found!"));
        }
    }

    //------------------------------------------Upload File---------------------------------------//
    public async Task UploadFileAsync(UploadFileDto file, CancellationToken ct)
    {
        bool bucketExists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(file.Folder), ct);
        if (bucketExists == false)
        {
            await CreateBucketWithLifecyclePolicy(file.Folder, ct);
        }
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(file.Folder)
            .WithObject(file.StoredName)
            .WithStreamData(file.Stream)
            .WithObjectSize(file.Size);

        if (!string.IsNullOrEmpty(file.MimeType))
            putObjectArgs = putObjectArgs.WithContentType(file.MimeType);

        await _client.PutObjectAsync(putObjectArgs, ct);
        _logger.LogInformation("Uploaded file {Name} to MinIO bucket {Folder}", file.StoredName, file.Folder);
    }

    //--------------------------------------Upload File With Retry--------------------------------//
    public async Task UploadFileWithRetryAsync(UploadFileDto file, CancellationToken ct)
    {
        var retryPolicy = Policy.Handle<MinioException>().WaitAndRetryAsync(
            _minioOptions.FileRetryCount,
            retryAttempt =>
                TimeSpan.FromMilliseconds(_minioOptions.FileRetryDelayMilliseconds * Math.Pow(2, retryAttempt)),
            ((exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning("Retry {retryCount}: Failed to upload file '{Name}' to bucket" +
                    " '{Folder}'. Waiting {timeSpan} before retrying.Exception:{exception}",
                    retryCount, file.OriginalName, file.Folder, timeSpan, exception);
            }));
        await retryPolicy.ExecuteAsync((Func<Task>)(async () =>
        {
            await UploadFileAsync(file, ct);
        }));
    }

    //------------------------------------------Upload File List-----------------------------------//
    public async Task<Result<List<FileUploadResponse>>> UploadFilesAsync(
        List<UploadFileDto> files,
        CancellationToken ct)
    {
        var minioFilesHandler = new MinioFilesHandler(_logger, _minioOptions.CountForSemaphore, ct);

        var uploadResult = await minioFilesHandler.ProcessFilesAsync(files, UploadFileWithRetryAsync);

        if (uploadResult.Data == null || uploadResult.Data.Count == 0)
            return UnitResult.Fail(uploadResult.Error);

        var response = new List<FileUploadResponse>();

        foreach (var file in files)
        {
            if (uploadResult.Data.Contains(file.StoredName))
            {
                FileUploadResponse uploadResponse = new(file.OriginalName, file.StoredName, true, string.Empty);
                response.Add(uploadResponse);
            }
            else
            {
                FileUploadResponse uploadResponse = new(file.OriginalName, file.StoredName, false, string.Empty);
                response.Add(uploadResponse);
            }
        }
        return Result.Ok(response);
    }

    //--------------------------------------------Delete File-------------------------------------//
    /// <summary>
    /// This method deletes a file from the MinIO bucket.Needs To be Modified for hard delete
    /// When bucket is versioned , the file is not deleted but a delete marker is created.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task DeleteFileAsync(FileDto file, CancellationToken cancelToken)
    {
        bool bucketExists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(file.Folder), cancelToken);
        if (!bucketExists)
        {
            _logger.LogWarning("Bucket '{BucketName}' does not exist. Cannot delete file '{FileName}'",
                file.Folder, file.Name);
            throw new InvalidOperationException($"Bucket '{file.Folder}' does not exist.");
        }
        //DELETE ALL VERSIONS of FILE FOR HARD DELETE
        var removeArgs = new RemoveObjectArgs().WithBucket(file.Folder).WithObject(file.Name);

        await _client.RemoveObjectAsync(removeArgs, cancelToken);

        _logger.LogInformation(
            "Deleted file {ObjectName} from MinIO bucket {BucketName}", file.Name, file.Folder);
    }

    //---------------------------------------Delete File with retry------------------------------//
    public async Task DeleteFileWithRetryAsync(FileDto file, CancellationToken ct)
    {
        var retryPolicy = Policy.Handle<MinioException>().WaitAndRetryAsync(_minioOptions.FileRetryCount,
            retryAttempt =>
                TimeSpan.FromMilliseconds(_minioOptions.FileRetryDelayMilliseconds * Math.Pow(1, retryAttempt)),
            (exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning("Retry {RetryCount}: Failed to delete file '{Name}' to bucket" +
                    " '{bucketName}'. Waiting {timeSpan} before retrying.",
                    retryCount, file.Name, file.Folder, timeSpan);
            });

        await retryPolicy.ExecuteAsync(async () =>
        {
            await DeleteFileAsync(file, ct);
        });
    }

    //-------------------------------------------Delete FileList----------------------------------//
    public async Task<Result<List<FileDeleteResponse>>> DeleteFileListAsync(
        List<FileDto> files,
        CancellationToken ct)
    {
        var minioFilesHandler = new MinioFilesHandler(_logger, _minioOptions.CountForSemaphore, ct);

        var deleteResult = await minioFilesHandler.ProcessFilesAsync(files, DeleteFileWithRetryAsync);

        if (deleteResult.Data == null || deleteResult.Data.Count == 0)
            return UnitResult.Fail(deleteResult.Error);

        var response = new List<FileDeleteResponse>();

        foreach (var file in files)
        {
            if (deleteResult.Data.Contains(file.Name))
            {
                FileDeleteResponse uploadResponse = new(file.Name, true);
                response.Add(uploadResponse);
            }
            else
            {
                FileDeleteResponse uploadResponse = new(file.Name, false);
                response.Add(uploadResponse);
            }
        }
        return Result.Ok(response);
    }

    //-------------------------------------Soft delete file --------------------------------------//
    public async Task SoftDeleteFileAsync(FileDto file, CancellationToken ct)
    {
        bool bucketExists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(file.Folder), ct);
        if (bucketExists == false)
        {
            _logger.LogWarning("Bucket '{Folder}' does not exist. Cannot delete file '{Name}'",
                file.Folder, file.Name);
            throw new InvalidOperationException($"Bucket '{file.Folder}' does not exist.");
        }
        var removeArgs = new RemoveObjectArgs()
            .WithBucket(file.Folder)
            .WithObject(file.Name);

        await _client.RemoveObjectAsync(removeArgs, ct);

        _logger.LogInformation("File '{Name}' in bucket '{Folder}' has been marked as deleted",
            file.Name, file.Folder);
    }

    //---------------------------------Soft delete file with retry--------------------------------//
    public async Task SoftDeleteFileWithRetryAsync(FileDto file, CancellationToken ct)
    {
        var retryPolicy = Policy.Handle<MinioException>().WaitAndRetryAsync(
            _minioOptions.FileRetryCount,
            retryAttempt =>
                TimeSpan.FromMilliseconds(_minioOptions.FileRetryDelayMilliseconds * Math.Pow(2, retryAttempt)),
            (exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning("Retry {retryCount}: Failed soft delete file '{Name}' in " +
                    "bucket '{Folder}'. Waiting {timeSpan} before retrying.",
                    retryCount, file.Name, file.Folder, timeSpan);
            });

        await retryPolicy.ExecuteAsync(async () => await SoftDeleteFileAsync(file, ct));
    }

    //------------------------------------Soft delete file list-----------------------------------//    
    public async Task<Result<List<string>>> SoftDeleteFilesAsync(
        List<FileDto> files, CancellationToken ct)
    {
        var minioFilesHandler = new MinioFilesHandler(_logger, _minioOptions.CountForSemaphore, ct);

        return await minioFilesHandler.ProcessFilesAsync(files, SoftDeleteFileWithRetryAsync);
    }

    //--------------------------------------Restore file------------------------------------------//
    public async Task RestoreFileAsync(FileDto file, CancellationToken ct)
    {
        bool bucketExists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(file.Folder), ct);
        if (bucketExists == false)
        {
            _logger.LogWarning("Bucket '{BucketName}' does not exist.Cannot restore file '{FileName}'",
                file.Folder, file.Name);

            throw new InvalidOperationException($"Bucket '{file.Folder}' does not exist.");
        }
        var response = await _amazonClient.ListVersionsAsync(file.Folder, file.Name, ct);

        foreach (var obj in response.Versions)
        {
            Console.WriteLine($"File is DeletedMarker {obj.IsDeleteMarker}, versionId:{obj.VersionId}");
            if (obj.IsDeleteMarker!= null && obj.IsDeleteMarker.Value == true)
            {
                var deleteMarkerArgs = new RemoveObjectArgs()
                    .WithBucket(file.Folder)
                    .WithObject(file.Name)
                    .WithVersionId(obj.VersionId);

                await _client.RemoveObjectAsync(deleteMarkerArgs, ct);

                _logger.LogInformation("File '{Name}' in bucket '{Folder}' has been restored",
                    file.Name, file.Folder);
            }
        }
    }

    public async Task RestoreFileAsyncV2(FileDto file, CancellationToken ct)
    {
        bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonClient, file.Folder);
        if (!bucketExists)
        {
            _logger.LogWarning("Bucket '{BucketName}' does not exist. Cannot restore file '{FileName}'"
                , file.Folder, file.Name);
            throw new InvalidOperationException($"Bucket '{file.Folder}' does not exist.");
        }
        var request = new ListVersionsRequest
        {
            BucketName = file.Folder,
            Prefix = file.Name
        };

        var response = await _amazonClient.ListVersionsAsync(request, ct);

        foreach (var obj in response.Versions)
        {
            if (obj.IsDeleteMarker != null && obj.IsDeleteMarker.Value == true)
            {
                var deleteMarkerArgs = new DeleteObjectRequest
                {
                    BucketName = file.Folder,
                    Key = file.Name,
                    VersionId = obj.VersionId
                };


                await _amazonClient.DeleteObjectAsync(deleteMarkerArgs, ct);

                _logger.LogInformation("File '{FileName}' in bucket '{BucketName}' has been restored.", file.Name, file.Folder);
            }
        }
    }

    //-----------------------------------Restore file with retry----------------------------------//
    public async Task RestoreFileWithRetryAsync(FileDto file, CancellationToken ct)
    {
        var retryPolicy = Policy.Handle<MinioException>().WaitAndRetryAsync(
            _minioOptions.FileRetryCount,
            retryAttempt =>
                TimeSpan.FromMilliseconds(_minioOptions.FileRetryDelayMilliseconds * Math.Pow(2, retryAttempt)),
            (exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning("Retry {RetryCount}: Failed to Restore file '{Name}' in " +
                    "bucket '{bucketName}'. Waiting {timeSpan} before retrying.",
                    retryCount, file.Name, file.Folder, timeSpan);
            });
        await retryPolicy.ExecuteAsync(async () => await RestoreFileAsync(file, ct));
    }

    //-----------------------------------Restore file list----------------------------------------//
    public async Task<Result<List<string>>> RestoreFilesAsync(
        List<FileDto> files,
        CancellationToken ct)
    {
        var minioFilesHandler = new MinioFilesHandler(_logger, _minioOptions.CountForSemaphore, ct);
        return await minioFilesHandler.ProcessFilesAsync(files, RestoreFileWithRetryAsync);
    }

    //----------------------------------RollBack files--------------------------------------------//
    public async Task<UnitResult> RollBackFilesAsync(
       List<FileDto> filesToRestore,
       List<FileDto> filesToDelete,
       CancellationToken ct)
    {
        var errorMessage = string.Empty;
        var restoreResult = await RestoreFilesAsync(filesToRestore, ct);
        if (restoreResult.IsFailure)
        {
            errorMessage = restoreResult.Error.Message;
            _logger.LogCritical("Fail restore some files! Error:{Message}",
                restoreResult.Error.Message);
        }
        var deleteResult = await DeleteFileListAsync(filesToDelete, ct);
        if (deleteResult.IsFailure)
        {
            errorMessage = errorMessage + ", " + restoreResult.Error.Message;
            _logger.LogCritical("Fail delete some files!Error:{Message}",
             deleteResult.Error.Message);
        }
        return string.IsNullOrEmpty(errorMessage)
            ? UnitResult.Fail(Error.InternalServerError(errorMessage))
            : UnitResult.Ok();
    }


    //MinIO uses a scanner process to check objects against all configured lifecycle management
    //rules. Slow scanning due to high IO workloads or limited system resources may delay application
    //of lifecycle management rules. See Lifecycle Management Object Scanner for more information.

    /// <summary>
    /// This method create a bucket with lifecycle policy
    /// </summary>
    /// <param name="bucket"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    private async Task CreateBucketWithLifecyclePolicy(string bucket, CancellationToken ct)
    {
        _logger.LogWarning("Bucket '{Folder}' does not exist!Creating bucket...", bucket);

        await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket), ct);

        var setVersioningArgs = new SetVersioningArgs().WithBucket(bucket).WithVersioningEnabled();

        await _client.SetVersioningAsync(setVersioningArgs, ct);

        var rules = new List<LifecycleRule>(){
                //delete noncurrent object version after '_minioOptions.ExpirationDays' days
                new LifecycleRule(
                    abortIncompleteMultipartUpload: null,
                    id: "DeleteAfter1Day",
                    expiration: null,
                    transition: null,
                    filter: null,
                    noncurrentVersionExpiration: new NoncurrentVersionExpiration
                    { NoncurrentDays = _minioOptions.ExpirationDays},
                    noncurrentVersionTransition: null,
                    status: "Enabled"),
                //delete deleteMarker if object is deleted
                new LifecycleRule(
                    abortIncompleteMultipartUpload: null,
                    id: "DeleteMarkers",
                    expiration: new Minio.DataModel.ILM.Expiration {ExpiredObjectDeleteMarker = true },
                    transition: null,
                    filter: null,
                    noncurrentVersionExpiration: null,
                    noncurrentVersionTransition: null,
                    status: "Enabled"
                )};

        var lifecyclePolicy = new LifecycleConfiguration(new List<LifecycleRule>(rules));

        var lifeCirclePolicyArgs = new SetBucketLifecycleArgs()
            .WithBucket(bucket)
            .WithLifecycleConfiguration(lifecyclePolicy);

        await _client.SetBucketLifecycleAsync(lifeCirclePolicyArgs, ct);
        _logger.LogWarning("Bucket with name'{Folder}' created, versioning is enabled, set" +
            " rules: 'DeleteAfter1Day','DleteMarkers'", bucket);
    }
}


