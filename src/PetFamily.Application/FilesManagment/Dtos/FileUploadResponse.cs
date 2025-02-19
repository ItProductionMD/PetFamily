namespace PetFamily.Application.FilesManagment.Dtos;

public class FileUploadResponse(string originalName, string storedName)
{
    public string OriginalName { get; private set; } = originalName;
    public string StoredName { get; private set; } = storedName;
    public bool IsUploaded { get; set; }
    public string Error { get; set; } = string.Empty;
}
