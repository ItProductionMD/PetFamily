using FileStorage.Public.Contracts;

namespace Volunteers.Application;

public class PetImagesValidatorOptions : IFileValidatorOptions
{
    public long MaxSize { get; set; } = 10 * 1024 * 1024; // 10 MB  
    public int MaxFilesCount { get; set; } = 10;
    public string[] AllowedExtensions { get; set; } = new[]
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff"
    };
    public string[] AllowedMimeTypes { get; set; } = new[]
    {
        "image/jpeg", "image/png", "image/gif", "image/bmp", "image/tiff"
    };
}
