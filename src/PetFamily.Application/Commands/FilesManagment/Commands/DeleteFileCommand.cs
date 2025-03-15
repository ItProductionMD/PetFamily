namespace PetFamily.Application.Commands.FilesManagment.Commands;

public class DeleteFileCommand
{
    public string StoredName { get; init; }
    public DeleteFileCommand(string name)
    {
        StoredName = name;
    }
}


