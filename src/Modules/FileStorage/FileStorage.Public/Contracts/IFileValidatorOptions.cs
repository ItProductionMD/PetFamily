namespace FileStorage.Public.Contracts;

public interface IFileValidatorOptions
{
    long MaxSize { get; set; } //bytes
    int MaxFilesCount { get; set; }
    string[] AllowedExtensions { get; set; }
    string[] AllowedMimeTypes { get; set; }
}
