using PetFamily.Application.Commands.FilesManagment.Commands;
using PetFamily.Application.Commands.FilesManagment.Dtos;

namespace PetFamily.API.Common.Utilities;

public static class Mappers
{
    public static List<UploadFileCommand> IFormFilesToUploadCommands(
        List<IFormFile> files,
        AsyncDisposableCollection disposableStreams)
    {
        var uploadCommands = new List<UploadFileCommand>();
        foreach (var file in files)
        {
            var stream = file.OpenReadStream();
            disposableStreams.Add(stream);

            var command = new UploadFileCommand(
                file.FileName,
                file.ContentType.ToLower(),
                file.Length,
                stream);

            uploadCommands.Add(command);
        }
        return uploadCommands;
    }

    public static List<AppFileDto> IFormFilesToAppFileDtos(
        List<IFormFile> files,
        string path,
        AsyncDisposableCollection disposableStreams)
    {
        var fileDtos = new List<AppFileDto>();
        foreach (var file in files)
        {
            var stream = file.OpenReadStream();
            disposableStreams.Add(stream);

            var command = new UploadFileCommand(
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

    public static AppFileDto IFormFileToAppFileDto(IFormFile file, string path, Stream stream)
    {
        var command = new UploadFileCommand(
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
