using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.AspNetCore.Http;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Validations;

namespace PetFamily.Framework.FormFiles;

public static class FormFileValidator
{
    public static UnitResult ValidateFiles(
        List<IFormFile> files,
        IFileValidatorOptions fileValidatorOptions)
    {

        if (files.Count > fileValidatorOptions.MaxFilesCount)
            return UnitResult.Fail(Error.ValueOutOfRange(ValidationErrorCodes.FILES_COUNT_OUT_OF_RANGE));

        List<Error> errors = [];
        foreach (var file in files)
        {
            if (file == null || file.Length == 0)
            {
                errors.Add(Error.FileValidation(
                    file?.FileName ?? "empty", ValidationErrorCodes.FILE_IS_EMPTY));

                break;
            }
            var fileExtension = UploadFileDto.GetFullExtension(file.FileName);
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
       IFileValidatorOptions fileValidatorOptions)
    {
        if (file == null || file.Length == 0)
            return UnitResult.Fail(Error.FileValidation(
                file?.FileName ?? "empty", ValidationErrorCodes.FILE_IS_EMPTY));

        List<Error> errors = [];

        var fileExtension = UploadFileDto.GetFullExtension(file.FileName);
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
