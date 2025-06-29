namespace FileStorage.Public.Dtos;

public record UploadFileDto(
    string OriginalName,
    string StoredName,
    string Extension,
    string MimeType,
    long Size,
    Stream? Stream,
    string Folder);


