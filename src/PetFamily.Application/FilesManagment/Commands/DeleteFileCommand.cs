namespace PetFamily.Application.FilesManagment.Commands;

public class DeleteFileCommand
{
    public string StoredName { get; init; }
    public DeleteFileCommand(string name)
    {
        StoredName = name;
    }
}


