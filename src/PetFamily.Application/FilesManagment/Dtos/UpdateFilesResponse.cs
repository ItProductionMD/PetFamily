namespace PetFamily.Application.FilesManagment.Dtos;

public record UpdateFilesResponse(
    List<FileDeleteResponse> DeleteResponse,
    List<FileUploadResponse> UploadResponse);
