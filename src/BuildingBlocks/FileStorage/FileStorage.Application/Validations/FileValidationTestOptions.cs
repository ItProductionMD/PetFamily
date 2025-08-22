using FileStorage.Public.Contracts;

namespace FileStorage.Application.Validations;

public class FileValidationTestOptions : IFileValidatorOptions
{
    public long MaxSize { get; set; } = 10 * 1024 * 1024; // 1 MB
    public int MaxFilesCount { get; set; } = 10;
    public string[] AllowedExtensions { get; set; } = [".jpg", ".png", ".jpeg"];
    public string[] AllowedMimeTypes { get; set; } = ["image/jpeg", "image/png", "image/gif"];
}
