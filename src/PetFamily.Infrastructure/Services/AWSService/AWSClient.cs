using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Infrastructure.Services.MinioService;

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
            ServiceURL = "http://"+minioOptions.Endpoint,
            ForcePathStyle = true
        };

        var s3Client = new AmazonS3Client(minioOptions.AccessKey, minioOptions.SecretKey, config);

        services.AddSingleton<IAmazonS3>(s3Client);

        return services;
    }
}


/* Пример запроса списка объектов
 var listRequest = new ListObjectsV2Request
{
    BucketName = "your-bucket"
};

var response = await s3Client.ListObjectsV2Async(listRequest);

foreach (var obj in response.S3Objects)
{
    Console.WriteLine($"File: {obj.Key}, Size: {obj.Size}");
}
*/