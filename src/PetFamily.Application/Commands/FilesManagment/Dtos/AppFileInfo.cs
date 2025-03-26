namespace PetFamily.Application.Commands.FilesManagment.Dtos;

public record AppFileInfo(string Name,string FolderName)
{
    public static List<AppFileInfo> CreateFromAppFileList(List<AppFileDto> appFiles)
    {
        return appFiles.Select(f => new AppFileInfo(f.Name, f.Folder)).ToList();
    }
}
