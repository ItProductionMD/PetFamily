using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.FilesManagment.Commands;

public class UploadFileCommand
{
    public string OriginalName { get; init; }
    public string StoredName { get; private set; }
    public string Extension { get; private set; }
    public string MimeType { get; private set; }
    public long Size { get; private set; }
    public Stream Stream { get; private set; }
    public UploadFileCommand(
        string originalName,
        string mimeType,
        long fileSize,
        string extension,
        Stream stream)
    {
        OriginalName = originalName;
        StoredName = string.Concat(Guid.NewGuid(), GetFullExtension(OriginalName));
        MimeType = mimeType;
        Size = fileSize;
        Stream = stream;
        Extension = extension;
    }
    public static string GetFullExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        int firstDotIndex = fileName.IndexOf('.');
        return firstDotIndex >= 0 ? fileName.Substring(firstDotIndex).ToLower() : string.Empty;
    }
}


