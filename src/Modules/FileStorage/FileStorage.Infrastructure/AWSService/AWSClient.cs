using Amazon.S3;
using FileStorage.Infrastructure.MinioService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorage.Infrastructure.AWSService;
public static class AWSConfiguration
{
    public static IServiceCollection AddAWSClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var minioOptions = configuration.GetSection("MinioOptions").Get<MinioOptions>()
            ?? throw new ApplicationException("Minio configuration wasn't found!");

        var config = new AmazonS3Config
        {
            ServiceURL = "http://" + minioOptions.Endpoint,
            ForcePathStyle = true
        };

        var s3Client = new AmazonS3Client(minioOptions.AccessKey, minioOptions.SecretKey, config);

        services.AddSingleton<IAmazonS3>(s3Client);

        return services;
    }
}