namespace FileStorage.Public.Dtos;

public record UpdateFilesResponse(
    List<FileDeleteResponse> DeleteResponse,
    List<FileUploadResponse> UploadResponse);
