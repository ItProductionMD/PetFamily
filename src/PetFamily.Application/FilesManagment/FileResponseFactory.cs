using Microsoft.AspNetCore.Localization;
using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Application.FilesManagment.Dtos;

namespace PetFamily.Application.FilesManagment;

public static class FileResponseFactory
{
    public static List<FileDeleteResponse> CreateDeleteResponseList(
        List<string> fileNames,
        List<DeleteFileCommand> commands)
    {
        return commands.Select(command =>
            new FileDeleteResponse(command.StoredName, fileNames.Contains(command.StoredName))).ToList();
    }

    public static List<FileUploadResponse> CreateUploadResponseList(
        List<string> uploadedFileNames,
        List<UploadFileCommand> commands)
    {
        return commands.Select(command => 
        {
            bool isUploaded = uploadedFileNames.Contains(command.StoredName);
            return new FileUploadResponse(command.OriginalName, command.StoredName)
            {
                IsUploaded = isUploaded,
                Error = isUploaded ? string.Empty : "Internal server error"
            };
        }).ToList();

    }

}
