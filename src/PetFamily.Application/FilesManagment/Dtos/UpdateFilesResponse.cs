namespace PetFamily.Application.FilesManagment.Dtos;

public record UpdateFilesResponse(
    List<FileDeleteResponse> deleteResponse,
    List<FileUploadResponse> uploadResponse);
