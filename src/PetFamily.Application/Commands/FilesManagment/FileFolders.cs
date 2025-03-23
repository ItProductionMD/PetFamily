using Microsoft.Extensions.Hosting;

namespace PetFamily.Application.Commands.FilesManagment;

public class FileFolders
{
    public string Images { get; init; } = string.Empty;
    public string Documents { get; init; } = string.Empty;
}
