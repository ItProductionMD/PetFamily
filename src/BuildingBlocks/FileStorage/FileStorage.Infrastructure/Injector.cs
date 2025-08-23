using FileStorage.Application;
using FileStorage.Application.IRepository;
using FileStorage.Application.Validations;
using FileStorage.Infrastructure.AWSService;
using FileStorage.Infrastructure.MessageQueue;
using FileStorage.Infrastructure.MinioService;
using FileStorage.Infrastructure.Scheduler;
using FileStorage.Public.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorage.Infrastructure;

public static class Injector
{
    public static IServiceCollection AddFileStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FileValidationTestOptions>(configuration.GetSection("FileValidators:Default"));


        services.AddSingleton<IUploadFileDtoValidator, UploadFileDtoValidator>();

        services.AddAWSClient(configuration)
                .AddMinio(configuration);

        services.AddSingleton<IFileMessageQueue, FileMessageQueue>()
                .AddScoped<IFileRepository, MinioFileRepository>()
                .AddScoped<IFileScheduler, FileScheduler>()
                .AddScoped<IFileService, FileService>();

        services.AddHostedService<MinioCleanupService>();

        return services;
    }
}
