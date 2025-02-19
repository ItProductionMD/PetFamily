using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;

namespace PetFamily.Application.SharedValidations;

public static class FileValidator
{
    public static UnitResult Validate(UploadFileCommand fileDto, FileValidatorOptions options)
    {
        List<Error> errors = [];

        if (fileDto.Stream == null || fileDto.Size == 0)
            errors.Add(Error.Custom(
                "file.is.invalid",
                "File is null or empty",
                ErrorType.Validation,
                "File"));

        if (fileDto.Size > options.MaxSize)
            errors.Add(Error.Custom(
                "file.is.invalid",
                $"The size of file is bigger than {options.MaxSize} !",
                ErrorType.Validation,
                "File"));

        if (!options.AllowedExtensions.Contains(fileDto.Extension))
            errors.Add(Error.Custom(
                "file.is.invalid",
                $"The file {fileDto.StoredName} format(extension) is not permited",
                ErrorType.Validation,
                "File"));

        if (!options.AllowedMimeTypes.Contains(fileDto.MimeType))
            errors.Add(Error.Custom(
               "file.is.invalid",
               $"The file {fileDto.StoredName} format(mimeType) is not permited",
               ErrorType.Validation,
               "File"));

        if (errors.Count > 0)
            return UnitResult.Fail(errors!);

        return UnitResult.Ok();
    }
    public static UnitResult ValidateMaxCount(int filesCount, FileValidatorOptions options)
    {
        if (filesCount > options.MaxFilesCount)
        {
            return UnitResult.Fail(Error.Custom(
                     "files.count.invalid",
                     $"Count of files is bigger than {options.MaxFilesCount}",
                     ErrorType.Validation,
                     "Files"));
        }
        return UnitResult.Ok();
    }


    /// <summary>
    /// Получение полного расширения файла (учитывает двойные расширения типа .tar.gz)
    /// </summary>
    
}
