namespace FileStorage.Public.Dtos;

public class DeleteFileDto
{
    public string StoredName { get; init; }
    public DeleteFileDto(string name)
    {
        StoredName = name;
    }
}


