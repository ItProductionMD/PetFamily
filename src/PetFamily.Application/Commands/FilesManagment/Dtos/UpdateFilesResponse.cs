namespace PetFamily.Application.Commands.FilesManagment.Dtos;

public record UpdateFilesResponse(
    List<FileDeleteResponse> DeleteResponse,
    List<FileUploadResponse> UploadResponse);
