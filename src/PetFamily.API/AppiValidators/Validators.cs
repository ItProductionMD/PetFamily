using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Application.SharedValidations;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;

namespace PetFamily.API.AppiValidators;

public static class Validators
{
    public static UnitResult ValidateFiles(
        List<IFormFile> files,
        FileValidatorOptions fileValidatorOptions)
    {
        List<Error> validationErrors = [];
        foreach (var file in files)
        {
            if (file == null || file.Length == 0)
            {
                validationErrors.Add(Error.Custom(
                    "file.is.invalid",
                    $"The file is null or empty!",
                    ErrorType.Validation,
                    "File"));
                break;
            }
            var fileExtension = UploadFileCommand.GetFullExtension(file.FileName);
            if (fileValidatorOptions.AllowedExtensions.Contains(fileExtension) == false)
                validationErrors.Add(Error.Custom(
                    "file.is.invalid",
                    $"The file {file.Name} format(extension) is not permited",
                    ErrorType.Validation,
                    "File"));

            if (fileValidatorOptions.AllowedMimeTypes.Contains(file.ContentType) == false)
                validationErrors.Add(Error.Custom(
                "file.is.invalid",
                   $"The file {file.Name} format(mimeType) is not permited",
                   ErrorType.Validation,
                "File"));

            if (file.Length > fileValidatorOptions.MaxSize)
                validationErrors.Add(Error.Custom(
                    "file.is.invalid",
                    $"The size of file is bigger than {fileValidatorOptions.MaxSize} !",
                    ErrorType.Validation,
                    "File"));
        }
        if (validationErrors.Count > 0)
            return UnitResult.Fail(validationErrors);

        return UnitResult.Ok();
    }
}
