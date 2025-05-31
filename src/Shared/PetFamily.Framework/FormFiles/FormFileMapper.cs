using FileStorage.Public.Dtos;
using Microsoft.AspNetCore.Http;
using PetFamily.Framework.Utilities;

namespace PetFamily.Framework.FormFiles;

public static class FormFileMapper
{
    public static List<UploadFileDto> ToUploadCommands(
        List<IFormFile> files,
        AsyncDisposableCollection disposableStreams)
    {
        var uploadCommands = new List<UploadFileDto>();
        foreach (var file in files)
        {
            var stream = file.OpenReadStream();
            disposableStreams.Add(stream);

            var command = new UploadFileDto(
                file.FileName,
                file.ContentType.ToLower(),
                file.Length,
                stream);

            uploadCommands.Add(command);
        }
        return uploadCommands;
    }

    public static List<FileDto> ToFileDtos(
        List<IFormFile> files,
        string path,
        AsyncDisposableCollection disposableStreams)
    {
        var fileDtos = new List<FileDto>();
        foreach (var file in files)
        {
            var stream = file.OpenReadStream();
            disposableStreams.Add(stream);

            var command = new UploadFileDto(
                file.FileName,
                file.ContentType.ToLower(),
                file.Length,
            stream);

            fileDtos.Add(
                new(
                command.StoredName,
                path,
                command.Stream,
                command.Extension,
                command.MimeType,
                command.Size)
                {
                    OriginalName = command.OriginalName
                });
        }
        return fileDtos;
    }

    public static FileDto ToFileDto(IFormFile file, string path, Stream stream)
    {
        var command = new UploadFileDto(
                file.FileName,
                file.ContentType.ToLower(),
                file.Length,
                stream);

        return new(
            command.StoredName,
            path,
            command.Stream,
            command.Extension,
            command.MimeType,
            command.Size)
        {
            OriginalName = command.OriginalName
        };
    }
}
