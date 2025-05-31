namespace FileStorage.Public.Dtos;

public class FileDto
{
    public string OriginalName { get; set; } = string.Empty;
    public string Name { get; set; }
    public Stream Stream { get; set; } = Stream.Null;
    public string Extension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Folder { get; set; } = string.Empty;
    public FileDto(string name, string path)
    {
        Name = name;
        Folder = path;
    }
    public FileDto(
        string name,
        string path,
        Stream stream,
        string extension,
        string mimeType,
        long size)
    {
        Name = name;
        Folder = path;
        Stream = stream;
        Extension = extension;
        MimeType = mimeType;
        Size = size;
    }

}
