namespace PetFamily.Application.Commands.FilesManagment.Dtos;

public class AppFileDto
{
    public string OriginalName { get; set; } = string.Empty;
    public string Name { get; set; }
    public Stream Stream { get; set; } = Stream.Null;
    public string Extension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Folder { get; set; } = string.Empty;
    public AppFileDto(string name,string path)
    {
        Name = name;
        Folder = path;
    }
	public AppFileDto(string name,string path,Stream stream, string extension, string mimeType,long size)
    {
        Name = name;
        Folder = path;
        Stream = stream;
        Extension = extension;
        MimeType = mimeType;
        Size = size;
    }

}
