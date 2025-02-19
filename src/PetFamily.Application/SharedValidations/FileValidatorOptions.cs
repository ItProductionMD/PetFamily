namespace PetFamily.Application.SharedValidations;

public class FileValidatorOptions
{
    public long MaxSize { get; set; } //bytes
    public int MaxFilesCount { get; set; }
    public string[] AllowedExtensions { get; set; } = [];
    public string[] AllowedMimeTypes { get; set; } = [];
}
