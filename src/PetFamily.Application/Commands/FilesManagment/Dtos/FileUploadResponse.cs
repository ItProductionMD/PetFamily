namespace PetFamily.Application.Commands.FilesManagment.Dtos;

public class FileUploadResponse(string originalName, string storedName)
{
    public string OriginalName { get; private set; } = originalName;
    public string StoredName { get; private set; } = storedName;
    public bool IsUploaded { get; set; }
    public string Error { get; set; } = string.Empty;

    public FileUploadResponse(
        string originalName,
        string storedName,
        bool isUploaded,
        string error) : this(originalName, storedName)
    {
        IsUploaded = isUploaded;
        Error = error;
    }
}
