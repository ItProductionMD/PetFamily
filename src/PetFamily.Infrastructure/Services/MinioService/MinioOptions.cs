using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Infrastructure.Services.MinioService
{
    public class MinioOptions
    {
        public const string MINIO = "Minio";
        public string Endpoint { get; init; } = string.Empty;
        public string AccessKey { get; init; } = string.Empty;
        public string SecretKey { get; init; } = string.Empty;
        public string BucketName { get; init; } = string.Empty;
        public bool WithSsl { get; init; }
    }
}
