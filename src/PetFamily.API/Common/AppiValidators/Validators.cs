using PetFamily.Application.Commands.FilesManagment.Commands;
using PetFamily.Application.SharedValidations;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.Validations;

namespace PetFamily.API.Common.AppiValidators;

public static class Validators
{
    public static UnitResult ValidateFiles(
        List<IFormFile> files,
        FileValidatorOptions fileValidatorOptions)
    {
        var validateCount = FileValidator.ValidateFilesCount(files.Count, fileValidatorOptions);
        if (validateCount.IsFailure)
            return validateCount;

        List<Error> errors = [];
        foreach (var file in files)
        {
            if (file == null || file.Length == 0)
            {
                errors.Add(Error.FileValidation(
                    file?.FileName ?? "empty", ValidationErrorCodes.FILE_IS_EMPTY));

                break;
            }
            var fileExtension = UploadFileCommand.GetFullExtension(file.FileName);
            if (fileValidatorOptions.AllowedExtensions.Contains(fileExtension) == false)
                errors.Add(Error.FileValidation(file.FileName, ValidationErrorCodes.FILE_INVALID_EXTENSION));

            if (fileValidatorOptions.AllowedMimeTypes.Contains(file.ContentType) == false)
                errors.Add(Error.FileValidation(file.FileName, ValidationErrorCodes.FILE_INVALID_MIME_TYPE));

            if (file.Length > fileValidatorOptions.MaxSize)
                errors.Add(Error.FileValidation(file.Name, ValidationErrorCodes.FILE_TOO_LARGE));
        }
        if (errors.Count > 0)
            return UnitResult.Fail(Error.FileValidation(errors));

        return UnitResult.Ok();
    }
    public static UnitResult ValidateFile(
       IFormFile file,
       FileValidatorOptions fileValidatorOptions)
    {
        if (file == null || file.Length == 0)
            return UnitResult.Fail(Error.FileValidation(
                file?.FileName ?? "empty", ValidationErrorCodes.FILE_IS_EMPTY));

        List<Error> errors = [];

        var fileExtension = UploadFileCommand.GetFullExtension(file.FileName);
        if (fileValidatorOptions.AllowedExtensions.Contains(fileExtension) == false)
            errors.Add(Error.FileValidation(file.FileName, ValidationErrorCodes.FILE_INVALID_EXTENSION));

        if (fileValidatorOptions.AllowedMimeTypes.Contains(file.ContentType) == false)
            errors.Add(Error.FileValidation(file.FileName, ValidationErrorCodes.FILE_INVALID_MIME_TYPE));

        if (file.Length > fileValidatorOptions.MaxSize)
            errors.Add(Error.FileValidation(file.Name, ValidationErrorCodes.FILE_TOO_LARGE));

        if (errors.Count > 0)
            return UnitResult.Fail(Error.FileValidation(errors));

        return UnitResult.Ok();
    }

}
