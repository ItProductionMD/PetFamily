namespace FileStorage.Public.Dtos;

public class UploadFileDto
{
    public string OriginalName { get; init; }
    public string StoredName { get; private set; }
    public string Extension { get; private set; }
    public string MimeType { get; private set; }
    public long Size { get; private set; }
    public Stream Stream { get; private set; }
    public UploadFileDto(
        string originalFileName,
        string mimeType,
        long fileSize,
        Stream stream)
    {
        var extension = GetFullExtension(originalFileName);
        OriginalName = originalFileName;
        StoredName = string.Concat(Guid.NewGuid(), extension);
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

    public FileDto ToAppFileDto(string path)
    {
        return new(
            StoredName,
            path,
            Stream,
            Extension,
            MimeType,
            Size);
    }
}


