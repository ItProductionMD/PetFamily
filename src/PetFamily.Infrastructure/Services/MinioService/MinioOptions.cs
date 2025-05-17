namespace PetFamily.Infrastructure.Services.MinioService
{
    public class MinioOptions
    {
        public string Endpoint { get; init; } = string.Empty;
        public string AccessKey { get; init; } = string.Empty;
        public string SecretKey { get; init; } = string.Empty;
        public string BucketName { get; init; } = string.Empty;
        public bool WithSsl { get; init; }
        public int CountForSemaphore { get; init; }
        public int FileRetryCount { get; init; }
        public int FileRetryDelayMilliseconds { get; init; }
        public uint ExpirationDays { get; init; }
    }
}
