using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.FilesManagment.Commands;

public class DeleteFileCommand : ICommand
{
    public string StoredName { get; init; }
    public DeleteFileCommand(string name)
    {
        StoredName = name;
    }
}


