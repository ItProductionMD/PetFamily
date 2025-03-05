using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Tracing;
using Minio.Handlers;
using System;

namespace PetFamily.Infrastructure.Services.MinioService;

public static class MinioConfiguration
{
    public static IServiceCollection AddMinio(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetSection("MinioOptions"));
        var logger = LoggerFactoryInstance.CreateLogger("MinioLogger");

        return services.AddMinio(options =>
        {
            var minioOptions = configuration.GetSection("MinioOptions").Get<MinioOptions>()
                ?? throw new ApplicationException("Minio configuration wasn't found!");

            options.WithEndpoint(minioOptions.Endpoint);
            options.WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);
            options.WithSSL(minioOptions.WithSsl);
            //options.SetTraceOn(new DefaultRequestLogger(logger)); //Gets an exception in GetObjectsEnumAsync
        });
    }
    public static readonly ILoggerFactory LoggerFactoryInstance = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });
}
