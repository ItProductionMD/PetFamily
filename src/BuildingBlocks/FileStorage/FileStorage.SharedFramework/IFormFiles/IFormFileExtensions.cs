using FileStorage.Public.Dtos;
using Microsoft.AspNetCore.Http;
using PetFamily.SharedKernel.Utilities;

namespace FileStorage.SharedFramework.IFormFiles;

public static class IFormFileExtensions
{
    public static string GetFullFileExtension(this IFormFile file)
    {
        if (string.IsNullOrWhiteSpace(file.FileName))
            return string.Empty;

        int firstDotIndex = file.FileName.IndexOf('.');

        return firstDotIndex >= 0
            ? file.FileName.Substring(firstDotIndex).ToLower()
            : string.Empty;
    }
    public static List<UploadFileDto> ToUploadFileDtos(
        this List<IFormFile> files,
        string Folder,
        AsyncDisposableCollection disposableStreams)
    {
        var uploadFileDtos = new List<UploadFileDto>();
        foreach (var file in files)
        {
            var extension = file.GetFullFileExtension();
            var storedName = string.Concat(Guid.NewGuid(), extension);
            var stream = file.OpenReadStream();
            disposableStreams.Add(stream);

            var uploadFileDto = new UploadFileDto(
                file.FileName,
                storedName,
                extension,
                file.ContentType.ToLower(),
                file.Length,
                stream,
                Folder);

            uploadFileDtos.Add(uploadFileDto);
        }
        return uploadFileDtos;
    }

    public static List<FileDto> ToFileDtos(
         this List<IFormFile> files,
         string path,
         AsyncDisposableCollection disposableStreams)
    {
        var fileDtos = new List<FileDto>();

        foreach (var file in files)
        {
            var stream = file.OpenReadStream();
            var extension = file.GetFullFileExtension();
            disposableStreams.Add(stream);
            fileDtos.Add(
                 new(
                     file.FileName,
                     path));
        }
        return fileDtos;
    }



    public static UploadFileDto ToUploadFileDto(this IFormFile file, string path, Stream? stream)
    {
        var extension = file.GetFullFileExtension();
        var storedName = string.Concat(Guid.NewGuid(), extension);

        return new UploadFileDto(
            file.FileName,
            storedName,
            extension,
            file.ContentType.ToLower(),
            file.Length,
            stream,
            path);
    }
}
