using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Domain.Shared.ValueObjects;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace PetFamily.Application.Commands.FilesManagment.Commands;

public class UploadFileCommand : ICommand
{
    public string OriginalName { get; init; }
    public string StoredName { get; private set; }
    public string Extension { get; private set; }
    public string MimeType { get; private set; }
    public long Size { get; private set; }
    public Stream Stream { get; private set; }
    public UploadFileCommand(
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
    public AppFileDto ToAppFileDto(string path)
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


