using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PetFamily.Infrastructure.Services.MinioService;

public class MinioFileService : IAppiFileService
{
    private readonly IMinioClient _client;
    private readonly ILogger<MinioFileService> _logger;
    public MinioFileService(IMinioClient client, ILogger<MinioFileService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Result<Uri>> GetFileUrl(
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
                // Проверяем, существует ли объект
                await _client.StatObjectAsync(new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName));

                // Если файл существует, генерируем ссылку
                var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithExpiry(60 * 60 * 24)); // 24 часа


                var uri = new Uri(url);

                return uri == null ? Result<Uri>.Failure(Error.CreateErrorNotFound("File in minio")) :
                    Result<Uri>.Success(uri);
            }
            catch (Exception ex)
            {
                _logger.LogError("File with name:{objectName} not found!Exception:{ex}",
                    objectName,ex);

                return Result<Uri>.Failure(Error.CreateErrorNotFound("File(minio) not found!"));
            }
        }
        _logger.LogError("Bucket with name:{bucketName} not found!", bucketName);
        return Result<Uri>.Failure(Error.CreateErrorNotFound("Bucket(minio) not found!"));
    }

    public async Task<Result<Guid>> RemoveFileAsync(
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
                await _client.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName), cancellationToken);

                return Result<Guid>.Success(Guid.Parse(objectName));
            }
            catch(Exception ex)
            {
                _logger.LogError("Error removing file with name:{objectName} " +
                    "from minio!Exeption:{ex.Message}",
                     objectName, ex);

                return Result<Guid>.Failure(Error.CreateErrorNotFound("File(minio) not found!"));
            }
        }
        return Result<Guid>.Failure(Error.CreateErrorNotFound("Bucket(minio) not found!"));
    }

    public async Task<Result<Guid>> UploadFileAsync(
        string bucketName,
        string objectName,
        Stream stream,
        CancellationToken cancellationToken)
    {
        var existArgs = new BucketExistsArgs().WithBucket(bucketName);
        var bucketExist = await _client.BucketExistsAsync(existArgs, cancellationToken);
        if (bucketExist)
        {
            try
            {
                await _client.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length),
                    cancellationToken);

                return Result<Guid>.Success(Guid.Parse(objectName));
            }
            catch (Exception ex) 
            {
                _logger.LogError("Write File in minio error!Exception:{ex}",ex);
                return Result<Guid>.Failure(Error.CreateErrorNotFound("Write File in minio error!"));
            }
        }
        return Result<Guid>.Failure(Error.CreateErrorNotFound("Bucket(minio) not found!"));
    }

}
